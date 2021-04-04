#version 330 core

layout (location = 0) out vec4 color;

in vec2 v_TextureCoordinates;

uniform vec4 u_Color;
uniform sampler2D u_Texture0;

void main()
{
    color = texture(u_Texture0, v_TextureCoordinates) * u_Color;
}
