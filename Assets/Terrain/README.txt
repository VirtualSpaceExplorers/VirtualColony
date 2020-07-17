
This is the "100km x 100km" east Hellas basin Nexus Aurora landing site on Mars.

The original terrain data (and updates) can be downloaded from:
	https://drive.google.com/drive/u/0/folders/1-A6Spug4YIiwQPQrvHKo5jJxi3ftbQlz

About the ".raw" terrain elevation file:
	HRSC_CTX_Hybrid_2049x2049.png is a 16-bit grayscale PNG
	Save as .pgm from GIMP
	Remove the binary pgm header using dd
		dd if=HRSC_mosaic_2049x2049.pgm of=HRSC_mosaic_2049x2049.data skip=1 bs=65
Import as 2049x2049, mac endian (big endian like PGM), flip vertically (PGM is top down).
Terrain size is 100000 x 100000 x 1500 (real) or 3000 (vertical exaggeration).

The Textures:
	- CTX_texture_8K is the Murray Lab CTX mosaic, approx 10m resolution, aligned with the terrain elevation file.
	- pebbles and sandy are seamlessly tiling 2K images used as detail textures
	- Mars Terrain Shader is a detail shader using the CTX, pebbles, and sandy textures.

Prepared by Dr. Orion Lawlor, lawlor@alaska.edu 2020-07 (Public Domain)


