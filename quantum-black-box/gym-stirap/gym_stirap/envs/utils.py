import numpy as np
from scipy.special import factorial, hermite

## Auxiliary functions
def gauss_x(x, sigma, x0, k0):
    """
    a gaussian wave packet of width sigma2, centered at x0, with momentum k0
    """
    return ((sigma * np.sqrt(np.pi)) ** (-0.5)
            * np.exp(-0.5 * ((x - x0) * 1. / sigma) ** 2 + 1j * x * k0))

def gauss_k(k, sigma, x0, k0):
    """
    analytical fourier transform of gauss_x(x), above
    """
    return ((sigma / np.sqrt(np.pi)) ** 0.5
            * np.exp(-0.5 * (sigma * (k - k0)) ** 2 - 1j * (k - k0) * x0))

def ho_eigenstate(x, m, omega, n, hbar = 1.):
    H = hermite(n)
    return ( 1 / np.sqrt(2**n * factorial(n)) * (m*omega / np.pi * hbar)**0.25 
            * np.exp(-m * omega * x**2 / (2 * hbar)) * 
            H(np.sqrt(m * omega / hbar) * x))