#include "sh_Utils.h"
#include "sh_TextureWrapping.h"

layout(location = 0) in highp vec2 v_Position;
layout(location = 1) in lowp vec4 v_Colour;
layout(location = 2) in highp vec2 v_TexCoord;
layout(location = 3) in highp vec4 v_TexRect;
layout(location = 4) in mediump vec2 v_BlendRange;
layout(location = 5) in highp float v_Result;

layout(std140, set = 0, binding = 0) uniform m_sliderParameters {
    highp vec2 centerPos;
    highp vec4 hitColor;
    highp float range;
    highp float fadeRange;
    bool reverse;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_texture;
layout(set = 1, binding = 1) uniform lowp sampler m_sampler;

layout(location = 0) out vec4 o_colour;

void main(void) 
{
    vec4 colour = toSRGB(v_Colour * wrappedSampler(wrap(v_TexCoord, v_TexRect), v_TexRect, m_texture, m_sampler, -0.9));

    vec2 diff = v_Position - centerPos;
    float dist = sqrt(diff.x * diff.x + diff.y * diff.y);

    if ( reverse != dist <= range ) 
    {
        o_colour = colour;
    } 
    else 
    {
        float progress = abs(dist - range) / fadeRange;
        o_colour = vec4(mix(vec3(1.0, 0.0, 0.0), hitColor.xyz, v_Result), mix(hitColor.w * colour.w, 0.0, progress));
    }
}
