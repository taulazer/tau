#include "sh_Utils.h"
#include "sh_TextureWrapping.h"

varying highp vec2 v_Position;
varying lowp vec4 v_Colour;
varying mediump vec2 v_TexCoord;
varying mediump vec4 v_TexRect;
varying mediump vec2 v_BlendRange;
varying highp float v_Result;

uniform highp vec2 centerPos;
uniform highp vec4 hitColor;
uniform highp float range;
uniform highp float fadeRange;
uniform highp bool reverse;
uniform lowp sampler2D m_Sampler;

void main(void) 
{
    vec4 colour = toSRGB(v_Colour * wrappedSampler(wrap(v_TexCoord, v_TexRect), v_TexRect, m_Sampler, -0.9));

    vec2 diff = v_Position - centerPos;
    float dist = sqrt(diff.x * diff.x + diff.y * diff.y);

    if ( reverse != dist <= range ) 
    {
        gl_FragColor = colour;
    } 
    else 
    {
        float progress = abs(dist - range) / fadeRange;
        gl_FragColor = vec4(mix(vec3(1.0, 0.0, 0.0), hitColor.xyz, v_Result), mix(hitColor.w * colour.w, 0.0, progress));
    }
}
