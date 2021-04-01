#version 330 core

layout (location = 0) out vec4 color;

in vec3 v_Position;
in vec2 v_TextureCoordinates;

uniform vec4 u_Color;

void main()
{
    color = u_Color;
}
