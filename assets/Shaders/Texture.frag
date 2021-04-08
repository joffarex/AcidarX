#version 450 core

layout (location = 0) out vec4 color;

in vec4 v_Color;
in vec2 v_TextureCoordinates;
in float v_TextureIndex;
in float v_TilingFactor;

uniform sampler2D u_Textures[32];

void main()
{
    int textureIndex = int(v_TextureIndex);
    
    color = texture(u_Textures[textureIndex], v_TextureCoordinates * v_TilingFactor) * v_Color;
//    color = v_Color;
}
