#version 400 core
layout (location = 0) in vec3 POSITION;
layout (location = 1) in vec4 COLOR;
layout (location = 2) in vec2 TEXCOORD;
layout (location = 3) in vec3 NORMAL;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 tangentToWorld;

smooth out vec4 p_color;
smooth out vec2 p_texcoords;
smooth out vec3 p_normal;
smooth out vec3 p_position;
smooth out vec3 p_vertexNormalWorldSpace;

void main()
{
	p_position =  (model * vec4(POSITION,1.0f)).xyz;
	gl_Position = projection * view * vec4(p_position,1.0);
	p_color = COLOR;
	p_texcoords = TEXCOORD;
	p_normal = NORMAL;
	p_vertexNormalWorldSpace = mat3(tangentToWorld) * NORMAL;
}