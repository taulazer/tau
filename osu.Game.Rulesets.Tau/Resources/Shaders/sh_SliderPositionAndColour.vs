attribute vec2 m_Position;
attribute vec4 m_Colour;
attribute mediump vec2 m_TexCoord;
attribute mediump vec4 m_TexRect;
attribute mediump vec2 m_BlendRange;
attribute float m_Result;

varying vec2 v_Position;
varying vec4 v_Colour;
varying mediump vec2 v_TexCoord;
varying mediump vec4 v_TexRect;
varying mediump vec2 v_BlendRange;
varying float v_Result;

uniform mat4 g_ProjMatrix;

void main(void)
{
    gl_Position = g_ProjMatrix * vec4(m_Position.xy, 1.0, 1.0);
    v_Position = m_Position;
    v_Colour = m_Colour;
    v_Result = m_Result;
    v_TexCoord = m_TexCoord;
    v_TexRect = m_TexRect;
    v_BlendRange = m_BlendRange;
}