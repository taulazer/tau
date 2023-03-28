layout(location = 0) in highp vec2 v_Position;
layout(location = 1) in lowp vec4 v_Colour;

layout(std140, set = 0, binding = 0) uniform m_maskParameters {
    highp vec2 aperturePos;
    highp vec2 apertureSize;
};

layout(location = 0) out vec4 o_colour;

const mediump float smoothness = 1.5;

// highp precision is necessary for vertex positions to prevent catastrophic failure on GL_ES platforms
lowp vec4 getColourAt(highp vec2 diff, highp vec2 size, lowp vec4 originalColour)
{
    highp float dist = length(diff);
    highp float radius = length(size);

    return originalColour * vec4(1.0, 1.0, 1.0, smoothstep(radius, radius * smoothness, dist));
}

void main(void)
{
    o_colour = mix(getColourAt(aperturePos - v_Position, apertureSize, v_Colour), vec4(0, 0.0, 0, 1.0), 0.0);
}
