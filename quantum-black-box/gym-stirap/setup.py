from setuptools import setup, find_packages

setup(name='gym-stirap',
      version='0.0.1',
      install_requires=['scipy', 'matplotlib'],
      packages=[package for package in find_packages()
                if package.startswith('gym')]
)  
