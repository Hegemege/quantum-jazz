# This files contains leftover code from the older solver
# TODO: Write a solver with the same interface as Scrhodinger

import numpy as np
from scipy.ndimage import _nd_image

        # We compute the kinetic matrix once and for all
        # self.K = self.kinematic_matrix(self.n) / (self.dx ** 2)

        # We need to evaluate the SE for smaller dt for each simulation dt
        # for i in range(self.internal_timesteps):
        #   self.psi = self.rk4(self.internal_dt, v, self.g, self.psi)
        # # Use Runge-Kutta scheme (more stable)
        
        # Use Euler scheme (Slightly Faster)
        # self.psi = self.eul(self.dt,v,self.g,self.psi)

#Function for the nonlinear term
def nonlin(self, psi):
        return -1j * self.g * np.conjugate(psi) * psi * psi

#Function for single time step evolution
def der(self, v, g, psi):
        # dpsi = 1j * np.dot(self.K, psi)
        
        # dpsi  = 1j * self.hbar ** 2 / (2 * self.mass) * np.gradient(np.gradient(psi, self.dx), self.dx)
        # dpsi[0] = 0.j
        # dpsi[-1] = 0.j
        
        dpsi = 1j * self.hbar ** 2 / (2 * self.mass) * fastLaplace1d(psi) / self.dx**2 # Faster than gradient, probably less accurate
        
        dpsi -= 1j * v * psi # Potential part
        if self.g > 0: 
            dpsi += self.nonlin(psi)
        return dpsi

#Euler scheme
def eul(self, h, v, g, psi):
        der1 = h * self.der(v, g, psi)
        psi += der1
        norm = self.norm(psi, self.x)
        if np.abs(1-norm) > 0.005:
                print('Becoming unstable, norm= ', norm)
        psi /=  norm
        return psi

# Runge-Kutta
def rk4(self, dt, v, g, psi):
        k1 = dt * self.der(v,g, psi)

        psi1 = self.psi + 0.5 * k1
        k2 = dt * self.der(v,g,psi1)

        psi2 = self.psi + 0.5 * k2
        k3 = dt * self.der(v,g,psi2)

        psi3 = self.psi + k3
        k4 = dt * self.der(v,g,psi3)

        psi += (k1 + 2*k2 + 2*k3 + k4)/6
        norm = self.norm(psi, self.x)

        if np.abs(1 - norm) > 0.005:
                print('Becoming unstable, norm= ', norm)
        psi /=norm
        return psi

laplace_filter = np.asarray([1, -2, 1], dtype=np.float64)

def fastLaplace1d(arr):
    """Evaluates the Laplacian operator for a 1D complex vector"""
    
    res = np.empty_like(arr)
    
    _nd_image.correlate1d(np.real(arr), laplace_filter, 0, np.real(res), 2, 0.0, 0)
    _nd_image.correlate1d(np.imag(arr), laplace_filter, 0, np.imag(res), 2, 0.0, 0)
    return res