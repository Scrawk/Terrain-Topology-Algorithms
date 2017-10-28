# Terrain-Topology-Algorithms

This project is collection of algorithms that can be used to describe the topology of a terrain. The result of these algorithms can then be used for rendering the terrain or as input to other algorithms

See [home page](https://www.digital-dust.com/single-post/2017/05/17/Terrain-topology-algorithms) for more information.

## Normal map

The normal map is probably the most common algorithm used to describe the topology of a terrain because of its use in rendering. The normal's are a tangent space vector of the surface direction.

Its a good place to start as it requires calculating the first order derivatives of the terrains height map which are also used in many of other algorithms.

![Normal Map](https://static.wixstatic.com/media/1e04d5_6f6f654ff3f3440f85a8f700d473caf1~mv2.jpg/v1/fill/w_550,h_550,al_c,q_80,usm_0.66_1.00_0.01/1e04d5_6f6f654ff3f3440f85a8f700d473caf1~mv2.jpg)

## Slope map

The slope map describes the steepness of the terrain and is handy for texturing the terrain. Like the normal map this also requires the first order derivatives.

![Slope Map](https://static.wixstatic.com/media/1e04d5_f86f6a25b15445dc9afff46625ea8ed1~mv2.jpg/v1/fill/w_550,h_550,al_c,q_80,usm_0.66_1.00_0.01/1e04d5_f86f6a25b15445dc9afff46625ea8ed1~mv2.jpg)

## Aspect map

This is similar to the slope map but instead represents the horizontal gradient as opposed to the vertical gradient.

The aspect map can take a few forms. The below image is of the terrains 'easterness'. The lighter areas face in the east direction and the darker in the west direction. The aspect can also represent 'northerness'. You can also just have the raw aspect value which is the faces angle in a 360 degree direction.

![Aspect Map](https://static.wixstatic.com/media/1e04d5_9743f6026a414409aba9f58af35b5039~mv2.jpg/v1/fill/w_550,h_550,al_c,q_80,usm_0.66_1.00_0.01/1e04d5_9743f6026a414409aba9f58af35b5039~mv2.jpg)

## Curvature map

The curvature map represents the convexity or concavity of the surface. Its a good way to get a cheap occlusion value for the terrain. The curvature is a little more complicated to calculate and requires the first and second order derivatives of the terrains height.

There are a lot ways to measure the curvature. Ive provided the vertical and horizontal curvature code as well as a average between them.

![Curvature Map](https://static.wixstatic.com/media/1e04d5_262a76b9cf404e1cbd5591c93bba2c92~mv2.jpg/v1/fill/w_550,h_550,al_c,q_80,usm_0.66_1.00_0.01/1e04d5_262a76b9cf404e1cbd5591c93bba2c92~mv2.jpg)

## Flow map

This one is a bit different as its a iterative algorithm and is therefore a bit slow. Its works by simulating the path a small amount of water flowing over the terrain will take. The magnitude of the waters velocity can then be used to create a flow map.

Its a good way to make rivers or erosion effects.

![Flow Map](https://static.wixstatic.com/media/1e04d5_89e146e844e44fb5a49549308f134886~mv2.jpg/v1/fill/w_550,h_550,al_c,q_80,usm_0.66_1.00_0.01/1e04d5_89e146e844e44fb5a49549308f134886~mv2.jpg)
