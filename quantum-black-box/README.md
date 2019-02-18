# Quantum Black Box

This repository contains the code for the Quantum Black Box, one of the two 
technological constraints of the [Quantum Wheel game jam 2019](http://www.finnishgamejam.com/quantumwheel/).

## Contents
- The python package `gym_stirap` (located in the folder `gym-stirap`) which 
  runs the simulation inside the black box. More details in the corresponding [README](gym-stirap/README.md).

- A Unity package containing a wrapper for the python package and a demo game to
show how to interact with the black box. The package contains a Pythonnet wrapper



## Requirements for the Unity package

- The Unity wrapper requires Unity 2018.3+, tested on Unity 2018.3.4 and on 
latest stable release
- Python 3.7.2 64bit installed
- Demo requires `TextMeshPro` essentials installed in Unity

## Installation

> This package has been tested with Python 3.7 64bit. Crashes with 32bit version
or previous Python versions have been reported.
> Multiple Python environments (e.g., Python 2.x and Python 3.x) can coexist in the same operating system. Please 
make sure that all the commands issued below are done in the Python 3.7 environment.

### Windows
1. Install Python 3.7. Download the 64-bit version from here https://www.python.org/ftp/python/3.7.2/python-3.7.2-amd64.exe 
    > The default download link in the download page points to the 32bit version, 
     that makes Unity crash. Please download the 64-bit version

    - Leave the default settings (optionally add python to PATH)
    - Other python distributions (Anaconda etc.) haven't been tested. 
    
2. Open a terminal in the `gym-stirap` directory

3. Initialize the Quantum Wheel library in Python by running the python command:
	
		python -m pip install ./
	
4. Set API compatibility level to .NET 4.x in Unity
- Edit → Project Settings → Player → Other Settings → Configuration → Api Compatibility Level

5. Install the unity package in Unity

6. Open the demo scene in Plugins/QuantumWheel/Scenes and run.

### Linux

1. Install the following packets
	- libpython3.7
	- python3.7
	- python3-pip
	- python3-tk (optional)

2. `cd gym-stirap`

3. Initialize the Quantum Wheel library in Python by running python command
	
        python -m pip install ./

4. Set API compatibility level to *.NET 4.x* in Unity
- Edit → Project Settings → Player → Other Settings → Configuration → Api Compatibility Level

5. Install the unity package in Unity

6. Open the demo scene in `Plugins/QuantumWheel/Scenes` and run.

### OSX

1. Install Python 3.7
	- https://www.python.org/downloads/release/python-372/

2. Unpack enclosed zip with the Python code to somewhere outside your unity project directory

3. Initialize the Quantum Wheel library in Python by running the command
	
    python -m pip install ./

4. Set API compatibility level to .NET 4.x in Unity
- Edit → Project Settings → Player → Other Settings → Configuration → Api Compatibility Level

5. Install the unity package in Unity

6. Open the demo scene in `Plugins/QuantumWheel/Scenes` and run.



Troubleshooting
===============

1. Unity throws some errors about TextMeshPro
- Make sure TextMeshPro essentials and extras are installed in Unity.
	- Unity should display a dialogue about installing these, so just click them, wait and close window. Clear log to see if errors persist.
	
2. Unity throws some errors about Python
- Make sure Python version is up to date 3.7.
	- Update Python
	- RESTART UNITY
- Make sure you ran the install_gym-stirap.py python script which is required for initialization.
	- Run script again
	- RESTART UNITY

3. Unity crashes to desktop (Windows)
    - Make sure that you have the standard distribution of python 3.7.2 installed
    - Make sure that it is the 64-bit version
    - Reinstall python as explained in this README and restart Unity.

Running builds made with Quantum Wheel plugin
=============================================
- Run steps 1-3
- You may include the gym-stirap folder/directory in your redistributable package, 
- but Python3.7 still needs to be installed and the library still needs to be initialized.


Manual installation from repository
===================================
- git clone https://gitlab.utu.fi/matros/gym-stirap
- cd gym-stirap
- git checkout quantum_wheel
- python -m pip install -e ./