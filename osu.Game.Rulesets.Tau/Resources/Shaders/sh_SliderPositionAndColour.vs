layout(location = 0) in highp vec2 m_Position;
layout(location = 1) in lowp vec4 m_Colour;
layout(location = 2) in highp vec2 m_TexCoord;
layout(location = 3) in highp vec4 m_TexRect;
layout(location = 4) in mediump vec2 m_BlendRange;
layout(location = 5) in highp float m_Result;

layout(location = 0) out vec2 v_Position;
layout(location = 1) out vec4 v_Colour;
layout(location = 2) out mediump vec2 v_TexCoord;
layout(location = 3) out mediump vec4 v_TexRect;
layout(location = 4) out mediump vec2 v_BlendRange;
layout(location = 5) out float v_Result;

void main(void)
{
    gl_Position = g_ProjMatrix * vec4(m_Position.xy, 1.0, 1.0);

    v_Position = m_Position;
    v_Colour = m_Colour;
    v_Result = m_Result;
    v_TexCoord = m_TexCoord;
    v_TexRect = m_TexRect;
    v_BlendRange = m_BlendRange;

    gl_Position.z = 0.0;
}