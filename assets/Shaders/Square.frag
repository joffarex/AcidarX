#version 330 core

layout (location = 0) out vec4 color;

in vec3 v_Position;
in vec4 v_Color;
in vec2 v_TextureCoordinates;

uniform sampler2D u_Texture0;

void main()
{
    color = v_Color * texture(u_Texture0, v_TextureCoordinates);
}
