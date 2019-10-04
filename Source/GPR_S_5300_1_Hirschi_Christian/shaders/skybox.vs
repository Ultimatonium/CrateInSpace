#version 400 core
layout (location = 0) in vec3 POSITION;
layout (location = 1) in vec3 COLOR;
layout (location = 2) in vec2 TEXCOORD;
layout (location = 3) in vec3 NORMAL;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

smooth out vec2 p_texcoords;

void main()
{
	gl_Position = projection * view * model * vec4(POSITION,1.0f);
	p_texcoords = TEXCOORD;
}