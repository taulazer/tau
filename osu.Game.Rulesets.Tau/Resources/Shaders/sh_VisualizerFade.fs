#include "sh_Utils.h"
#include "sh_Masking.h"
#include "sh_TextureWrapping.h"

layout(location = 5) in highp vec2 v_Position;
layout(location = 2) in mediump vec2 v_TexCoord;

layout(std140, set = 0, binding = 0) uniform m_visualizerParameters {
    highp vec2 centerPos;
    highp float range;
    highp float fadeRange;
};

layout(set = 0, binding = 1) uniform lowp texture2D m_texture;
layout(set = 0, binding = 2) uniform lowp sampler m_sampler;

layout(location = 0) out vec4 o_colour;

void main(void) 
{
    vec2 diff = v_Position - centerPos;
    float dist = sqrt(diff.x * diff.x + diff.y * diff.y);

    if ( dist <= range ) 
    {
        discard;
    } 
    else 
    {
        vec2 wrappedCoord = wrap(v_TexCoord, v_TexRect);
        vec4 colour = getRoundedColor(toSRGB(wrappedSampler(wrappedCoord, v_TexRect, m_texture, m_sampler, -0.9)), wrappedCoord);
        float progress = (fadeRange == 0.0) ? 1.0 : (abs(dist - range) / fadeRange);
        o_colour = vec4(colour.xyz, min(mix(0.0, colour.w, progress), colour.w));
    }
}