#include "sh_Utils.h"
#include "sh_Masking.h"
#include "sh_TextureWrapping.h"

varying highp vec2 v_Position;
varying mediump vec2 v_TexCoord;

uniform highp vec2 centerPos;
uniform highp float range;
uniform highp float fadeRange;
uniform lowp sampler2D m_Sampler;

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
        vec4 colour = getRoundedColor(toSRGB(wrappedSampler(wrappedCoord, v_TexRect, m_Sampler, -0.9)), wrappedCoord);
        float progress = (fadeRange == 0.0) ? 1.0 : (abs(dist - range) / fadeRange);
        gl_FragColor = vec4(colour.xyz, min(mix(0.0, colour.w, progress), colour.w));
    }
}