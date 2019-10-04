#version 400 core

uniform sampler2D baseColorMap;

smooth in vec2 p_texcoords;
out vec4 fragColor;

void main()
{
   fragColor = vec4(texture(baseColorMap, p_texcoords).rgb, 1.0f);
}