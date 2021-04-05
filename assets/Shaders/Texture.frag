#version 330 core

layout (location = 0) out vec4 color;

in vec2 v_TextureCoordinates;
in vec4 v_Color;

//uniform float u_TilingFactor;

//uniform sampler2D u_Texture0;

void main()
{
//    color = texture(u_Texture0, v_TextureCoordinates * u_TilingFactor) * v_Color;
    color = v_Color;
}
