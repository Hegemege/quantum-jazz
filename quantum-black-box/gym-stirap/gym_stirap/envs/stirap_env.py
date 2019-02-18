from gym_stirap.envs.spaces import Box

import numpy as np 

import matplotlib.pyplot as plt
from matplotlib.backends.backend_agg import FigureCanvasAgg as FigureCanvas
from mpl_toolkits.axes_grid1.inset_locator import inset_axes

# Schrodinger split-step fourier transform solver
from gym_stirap.envs.schrodinger import Schrodinger
from gym_stirap.envs.utils import gauss_x

class StirapEnv:
    """
        Description:
            A potential with three wells is used to confine a quantum system:

                v(x) = 0.5 * self.trap_strength * (x - xl)**2 *  x ** 2 * (x - xr)**2

            The dynamics is described by the 1D Schrodinger equation.

            The system is initially in the ground state of the left well. The goal is to move as much of the 
            probability density to the right well by the end of the dynamics.
            
            The agent can move the left and right wells left independently by an amount Delta at each timestep.

        Attributes:
            full_observation = False: if True, return the whole wave function
            timesteps = None: if given an integer value > 0, the number of timesteps 
                              before the dynamics is terminated. Otherwise, the dynamics 
                              continues indefinitely
                
        Observation: 
    
            If full_observation=False:
                Type: Box(4)
                Num	Observation                 Min         Max
                0	Left population               0           1
                1	Right population              0           1
                2	Left well position            -2          +2
                3	Right well position           -2          +2
                
            If full_observation=True:
                Type: Box(2 * n + 2)
                where n is the number of space points.

                (re(psi), im(psi), left_well_pos, right_well_pos)
                
        Actions:
                Type: Box(2)
                Num	Action                      Min         Max
                0   Move left well of amount     -1         +1
                1   Move right well of amount    -1         +1
                
        Reward:
                Reward at each time step is proportional to the population in the right well times t
                Reward -10 is given if the episode terminates before hand

        Starting State:
                The system is initially in the ground state of the left well. 

        Episode Termination:
                * After the amount of time defined by `timesteps`, if specified
                * If the agent moves the traps out of range (or swaps them)
                
        Solved Requirements:
                Handmade solution scores 120. Try and beat it
        """

    metadata = {'render.modes': ['human', 'rgb_array']}
    def __init__(self, final_reward=False, dt=0.0025, timesteps = None, initial_displacement = 0.):
        # Simulation parameters
        self.n = 512                  # Number of points in space
        self.xlim = 2
        self.x = np.linspace(-self.xlim, self.xlim, self.n) # Positions vector
        self.dx = self.x[1] - self.x[0]

        self.dt = dt
        
        self.timesteps = None
            
        if timesteps is not None:
            if timesteps > 0:
                self.timesteps = timesteps
        
        self.time = None

        self.mass = 2.
        self.hbar = 1.
        
        # The SE needs to be evaluated for a higher number of timesteps
        # in order to ensure convergence
        self.internal_timesteps = 101

        # This gives the ratio between dt and dx^2 
        # (for the solution to be stable, it has to be ~< 0.1)
        # print("dt/dx^2 ratio ", self.internal_dt / self.dx**2)
    
        # STIRAP parameters
        self.g = 0 # Nonlinear interaction term
        self.well_size = .5

        # Position of the wells
        self.left_center = -1.5 * self.well_size
        self.right_center = 1.5 * self.well_size
        
        # Strength of the potential
        self.trap_strength = 2.e3
        
        self.solver = None

        # Definition of the wells
        # These vectors are one where the corresponding well is located
        self.left_well = np.where(self.x < self.left_center + 2 * self.well_size / 3, 1, 0)
        self.right_well = np.where(self.x > self.right_center - 2 * self.well_size /3, 1, 0)
        
        # Initial state of the system
        # Initial state is the ground state of the left trap
        self.omega = np.sqrt(self.trap_strength/self.mass)

        self.psi0 = gauss_x(self.x, np.sqrt(self.hbar / (self.mass * self.omega)), self.left_center, 0)
        
        self.it = None
        self.il = None
        self.ir = None

        # RL parameters
        self.Delta = 0.01 # How much to change the well positions at each step

        self.initial_displacement = np.clip(initial_displacement, -.25, +.25)
        
        self.psi0 = gauss_x(self.x, np.sqrt(self.hbar / (self.mass * self.omega)), self.left_center + self.initial_displacement, 0)
        
        self.action_space = Box(low=np.array([-1.5, 0.]), 
                                high=np.array([0., 1.5]), 
                                dtype=np.float32)

        self.observation_space = Box(low=np.array([0, 0, - self.xlim, - self.xlim, 0]), 
                                high=np.array([1, 1, self.xlim, self.xlim, np.inf]),dtype=np.float32)

        # For the rendering
        self.viewer = None
        
        # Initializes the environment
        self.reset()

    def reset(self):
        """Reinitialize the system to the initial conditions"""
        self.psi = self.psi0.copy() # We need a deep copy
        self.il = self.left_center
        self.ir = self.right_center

        self.it = 0
        self.time = 0
        
        self.update_state()

        # Reinitialize the Schrodinger solver
        self.solver = Schrodinger(self.x, self.psi0, self.potential(), m= self.mass, hbar=self.hbar)

        return self.state
        
    def step(self, action):
        """ Performs a step of the evolution after the action specified.

        The function evaluates the Schrodinger Equation for dt and returns
        
        (state, reward, done, {})

        state: The state of the environment at t + dt (numpy.array)
        reward: A float corresponding to the reward function for the agent
        done: if the dynamics is complete
        {}: empty dict. Can possibly contain debug info
        """
        assert self.action_space.contains(action), "%r (%s) invalid"%(action, type(action))
        
        # Update the positions of the wells with the chosen action, 
        self.il = action[0]
        self.ir = action[1]

        # Update the potential
        self.solver.V_x = self.potential()
        
        # Update internal dt in case it gets changed
        self.internal_dt = self.dt / (self.internal_timesteps - 1)

        # Evaluate the state at time t + dt (internal timesteps are used to improve accuracy)
        self.solver.time_step(self.internal_dt, Nsteps = self.internal_timesteps)
        self.psi = self.solver.psi_x
        
        self.time += self.dt
        self.it += 1

        # Check if the simulation is done
        self.update_state()
        
        # Calculate the reward
        reward = self.reward()

        if (self.timesteps is not None) and self.it == self.timesteps:
            done = True
        else:
            done = False

            # We interrupt the simulation if the traps are moved too far away We punish the agent for moving the traps too far away, or for crossing them
            if self.il < (- self.xlim + 1 * self.well_size ) or self.il > 0:
                reward -= 10
                done = True
            if self.ir < 0 or (self.ir > self.xlim - 1 * self.well_size):
                reward -= 10
                done = True

        # Return the whole wavefunction as fourth parameter
        return self.state, reward, done, self.psi
        #return self.state, reward, done, np.real(self.psi), np.imag(self.psi)

    def update_state(self):
        """ Prepares the state to be passed to the agent"""
        # This is the state that the agent sees (left/right population, position of the wells)
        self.state = np.array(self.evaluate_populations() + (self.il, self.ir, self.time))
        return self.state

    def render(self, mode='human'):
        """Renders the system in a matplotlib figure and updates it at each timestep"""

        assert mode in ['human', 'rgb_array'], "mode is either 'human' or 'rgb_array'"
        # Probability density
        ys = np.abs(self.psi)**2 * self.dx

        if self.viewer is None:
            # Here we initialize the plot

            if mode == 'human':
                self.viewer = plt.figure()#figsize=[7, 5], dpi=72)
            elif mode == 'rgb_array':
                self.viewer = plt.figure(figsize=[7, 5], dpi=40)

            ax = self.viewer.gca()
            ax.spines['top'].set_visible(False)
            ax.spines['right'].set_visible(False)
            ax.spines['left'].set_visible(False)
            plt.yticks([], [])

            plt.fill_between(self.x, self.left_well, 'C0', alpha=.2)
            plt.fill_between(self.x, self.right_well, 
                        y2=np.zeros(self.right_well.shape), color='C1', alpha=.2)
            
            self.probability_line, = plt.plot(self.x, ys)
            self.potential_line, = plt.plot(self.x, 0.5*self.dx * self.potential(),'k')
            plt.ylim([0, .2])

            self.axins = inset_axes(ax, width="30%", height="60%", loc='upper left')
            #self.axins.set_xlim([0, self.totaltime])
            self.axins.set_ylim([-1, 1])
            self.axins.spines['bottom'].set_position('zero')
            self.axins.spines['top'].set_visible(False)
            self.axins.spines['right'].set_visible(False)
            self.axins.patch.set_alpha(0.5)

            self.axins.set_xlabel('t')
            self.leftpop_line, = self.axins.plot(self.dt * self.it, self.evaluate_populations()[0])
            self.rightpop_line, = self.axins.plot(self.dt * self.it, self.evaluate_populations()[1])
            self.centerpop_line, = self.axins.plot(self.dt * self.it, 1 - np.sum(self.evaluate_populations()))
            self.rightwell, = self.axins.plot(self.dt * self.it, self.ir, 'k--')
            self.leftwell, = self.axins.plot(self.dt * self.it, self.il, 'k--')

            #self.axins.legend(['p_r','left', 'right'])

            if mode == 'human':
                plt.pause(0.000001)
            elif mode == 'rgb_array':
                return self._to_rgbarray()
                
        # Here we update the plot
        self.probability_line.set_ydata(ys)
        self.potential_line.set_ydata(0.5*self.dx * self.potential())

        self.rightpop_line.set_xdata(np.append(self.rightpop_line.get_xdata(),self.time))
        self.rightpop_line.set_ydata(np.append(self.rightpop_line.get_ydata(),self.evaluate_populations()[1]))

        self.leftpop_line.set_xdata(np.append(self.leftpop_line.get_xdata(),self.time))
        self.leftpop_line.set_ydata(np.append(self.leftpop_line.get_ydata(),self.evaluate_populations()[0]))

        self.centerpop_line.set_xdata(np.append(self.centerpop_line.get_xdata(),self.time))
        self.centerpop_line.set_ydata(np.append(self.centerpop_line.get_ydata(),1 - np.sum(self.evaluate_populations())))

        self.rightwell.set_xdata(np.append(self.rightwell.get_xdata(),self.time))
        self.rightwell.set_ydata(np.append(self.rightwell.get_ydata(),self.ir))

        self.leftwell.set_xdata(np.append(self.leftwell.get_xdata(),self.time))
        self.leftwell.set_ydata(np.append(self.leftwell.get_ydata(),self.il))
        
        self.axins.set_xlim([0, self.time])
        if mode == 'human':
            plt.pause(0.000001)
        elif mode == 'rgb_array':
            return self._to_rgbarray()

    def _to_rgbarray(self):
        canvas = FigureCanvas(self.viewer)
        canvas.draw()       # draw the canvas, cache the renderer
        image = np.frombuffer(canvas.tostring_rgb(), dtype='uint8')
        image = image.reshape(canvas.get_width_height()[::-1] + (3,))
        return image

    def reward(self):
        """  Define the reward """
        reward = 0.
        
        (pop_left, pop_right) = self.evaluate_populations()
        reward = 2 * (pop_right * self.time) ** 2
    
        return reward

    # These are auxiliary functions for the physical system 

    def norm(self, psi, x):
        """ Norm of the wavefuncion psi defined on space points x"""
        return np.trapz(np.abs(psi)**2, x)

    def evaluate_populations(self):
        """ Returns the integral of |psi|^2 in the left and right well"""

        left = np.sum(np.abs(self.psi * self.left_well)**2) * self.dx
        right = np.sum(np.abs(self.psi * self.right_well)**2) * self.dx
        return (left, right)

    def potential(self):
        """ Returns the trapping potential """
        #self.il = np.clip(self.il, self.left_center - self.well_size / 2, self.left_center + self.well_size / 2)
        #self.ir = np.clip(self.ir, self.right_center - self.well_size / 2, self.right_center + self.well_size / 2)
        
        # v = np.zeros_like(self.x)
        v = 0.5 * self.trap_strength * ((self.x-self.il)**2) * (self.x ** 2) * ((self.x-self.ir)**2) 
        # + np.exp( (self.x-self.x[-1]-self.dx)** -2 ) + np.exp( (self.x-self.x[0]-self.dx)** -2)
        return v
