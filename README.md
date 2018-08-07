### **[Video](https://www.youtube.com/watch?v=qcqqkTYDUX8)**

![image](https://user-images.githubusercontent.com/5301844/43784547-099764e0-9a6d-11e8-8714-c68fef5a1297.png)
 
### Escape
`Escape` is the arcade game with `Fluids` simulation and smart `AI`.  
`MonoGame` Framework is used.  

### Fluids
Escape uses `2D Fluid Simulation` on `GPU` with `Vorticity Confinement` and `MacCormak Advection` scheme based on `Navier-Stokes` equations for incompressible flow. To solve them numerical methods are used.

* Because of the large amount of parallelism in graphics hardware, the simulation runs significantly faster on the `GPU` than on the `CPU`. Implementation is based on `HLSL` shaders, samplers and `render to texture` technique.

* To achieve higher-order accuracy, Escape uses a `MacCormack` scheme that performs two intermediate `semi-Lagrangian` advection steps.

* Also there are arbitrary boundaries conditions around obstacles implemented.
In 3D realization, the `Fluid` is rendered by `Ray-Casting` technique.

Fluid integration into gameplay mechanics performed on `CPU` is shown on the diagram below:

![image](https://cloud.githubusercontent.com/assets/5301844/2763364/ab0f65a6-ca02-11e3-86f4-f85336b6b9ab.png)

### Game Objects model
It is based on the Composition design pattern. It provides uniform interface to handle either single `Object` or entire `Objects` hierarchy:

![image](https://cloud.githubusercontent.com/assets/5301844/2763449/e7cc7604-ca03-11e3-94bf-bebff0aa94da.png)    
![image](https://cloud.githubusercontent.com/assets/5301844/2763461/08b8e122-ca04-11e3-97c2-daff2d9e2d74.png)     

### Drawing
Drawing of game objects is performed by the `Drawable` class. 
It is included into `Object` through composition.

![image](https://cloud.githubusercontent.com/assets/5301844/2763524/ba0587b4-ca04-11e3-9f5a-da9fed113f81.png)

### Modules
![image](https://cloud.githubusercontent.com/assets/5301844/2763539/fa59014c-ca04-11e3-88ad-bd98603547b7.png)
