#version 400 core

smooth in vec4 p_color;

out vec4 fragColor;

void main()
{
   fragColor = p_color;
}