﻿#version 330 core
layout (location = 0) in vec3 a_Position;

out vec3 v_Position;

uniform mat4 u_ViewProjection;
uniform mat4 u_Model;

void main()
{
    v_Position = a_Position;
    gl_Position = u_ViewProjection * u_Model * vec4(a_Position, 1.0);
}