import gym_stirap
import numpy as np
import warnings
import pytest

from gym_stirap.envs.utils import gauss_x, gauss_k, ho_eigenstate

# Test Schrodinger solver

######################
def test_schrodinger():
    n = 512                  # Number of points in space
    xlim = 2
    x = np.linspace(-xlim, xlim, n) # Positions vector
    potential = np.zeros(n)

    psi0 = gauss_x(x, .2, 0, 0)
    
    solver = gym_stirap.envs.schrodinger.Schrodinger(x, psi0, potential)
    assert solver is not None

    assert np.allclose(solver.psi_k, gauss_k(solver.k, .2, 0, 0))

    with pytest.warns(Warning):
        solver.time_step(.01, 10)
    
@pytest.mark.parametrize("n", [0, 1, 2])
def test_harmonic_oscillator(n):
    """
    Checks that the eigenstates of the harmonic oscillator are constant in time
    """
    nx = 512                 # Number of points in space
    m = 1.
    xlim = 2
    x = np.linspace(-xlim, xlim, nx) # Positions vector

    omega = 100.
    psi0 = (1. + 0.j) * ho_eigenstate(x, m=m, omega=omega, n=n)
    potential = 0.5 * omega**2 * m * x**2
    dt = 0.0001
    Nsteps = 1000
    solver = gym_stirap.envs.schrodinger.Schrodinger(x, psi0, potential, m = m)
    solver.time_step(dt, Nsteps=Nsteps)
    
    analytical_psi = np.exp(-1j*(2*n+1)*omega/2*solver.t)*psi0

    assert np.allclose(np.abs(solver.psi_x)**2, np.abs(analytical_psi)**2, rtol=10 * Nsteps * dt**2,atol= 10* Nsteps*dt**2)
    assert np.allclose(solver.psi_x, analytical_psi, rtol=20 * Nsteps * dt**2, atol= 20* Nsteps*dt**2)

def test_free_gaussian_packet():
    nx = 512                 # Number of points in space
    m = 1.
    xlim = 2
    x = np.linspace(-xlim, xlim, nx) # Positions vector
    k0 = 10
    omega = 100.

    # A Gaussian state with some right momentum
    psi0 = (1. + 0.j) * ho_eigenstate(x, m=m, omega=omega, n=0) * np.exp(1j * k0 * x)
    potential = np.zeros(x.shape)
    
    dt = 0.0001
    Nsteps = 300
    solver = gym_stirap.envs.schrodinger.Schrodinger(x, psi0, potential, m = m)
    solver.time_step(dt, Nsteps=Nsteps)
    t = solver.t
    analytical_psi = (np.exp((k0**2*t - 2*k0*x - 1j*m*omega*x**2)/(2j - 2*m*omega*t))/
      ((m*omega)**0.25*np.pi**0.25*np.sqrt(1/(m*omega) + 1j*t)))

    assert np.allclose(np.abs(solver.psi_x)**2, np.abs(analytical_psi)**2, rtol=10 * Nsteps * dt**2,atol= 10* Nsteps*dt**2)
    assert np.allclose(solver.psi_x, analytical_psi, rtol=10 * Nsteps * dt**2,atol= 10* Nsteps*dt**2)
