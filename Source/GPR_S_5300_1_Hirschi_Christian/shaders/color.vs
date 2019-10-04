#version 400 core
layout (location = 0) in vec3 POSITION;
layout (location = 1) in vec4 COLOR;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

smooth out vec4 p_color;

void main()
{
	gl_Position = projection * view * model * vec4(POSITION,1.0f);
	p_color = COLOR;
}