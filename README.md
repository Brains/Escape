###Escape
Escape is the arcade game with Fluids simulation and smart AI.  
**[Video](https://www.youtube.com/watch?v=qcqqkTYDUX8)**  
MonoGame Framework is used.  

###Fluids
Escape uses 2D Fluid Simulation on GPU with *Vorticity Confinement* and *MacCormak Advection* scheme based on *Navier-Stokes* equations for incompressible flow. To solve them numerical methods are used.

* Because of the large amount of parallelism in graphics hardware, the simulation runs significantly faster on the GPU than on the CPU. Implementation is based on HLSL shaders, samplers and *render to texture* technique.

* To achieve higher-order accuracy, Escape uses a *MacCormack* scheme that performs two intermediate *semi-Lagrangian* advection steps.

* Also there is implemented Arbitrary Boundaries conditions around obstacles.
In 3D realization, the Fluid is rendered by *Ray-Casting* technique.

On the diagram below is shown Fluid integration into gameplay mechanics performed on CPU:
![image](https://cloud.githubusercontent.com/assets/5301844/2763364/ab0f65a6-ca02-11e3-86f4-f85336b6b9ab.png)
