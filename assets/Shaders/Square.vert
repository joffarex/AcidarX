#version 330 core
layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec4 a_Color;
layout (location = 2) in vec2 a_TextureCoordinates;

out vec3 v_Position;
out vec4 v_Color;
out vec2 v_TextureCoordinates;

uniform mat4 u_ViewProjection;
uniform mat4 u_Model;

void main()
{
    v_Position = a_Position;
    v_Color = a_Color;
    v_TextureCoordinates = a_TextureCoordinates;
    gl_Position = u_ViewProjection * u_Model * vec4(a_Position, 1.0);
}