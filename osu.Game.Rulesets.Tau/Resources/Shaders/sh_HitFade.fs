varying highp vec2 v_Position;
varying lowp vec4 v_Colour;

uniform highp vec2 centerPos;
uniform highp vec4 hitColor;
uniform highp float range;
uniform highp float fadeRange;
uniform highp float alpha;

void main(void) 
{
    vec2 diff = v_Position - centerPos;
    float dist = sqrt(diff.x * diff.x + diff.y * diff.y);
    if ( dist <= range ) {
       gl_FragColor = vec4(v_Colour.xyz, alpha);
    }
    else {
       float progress = (dist - range) / fadeRange;
       gl_FragColor = vec4(mix(vec3(1.0, 0.0, 0.0), hitColor.xyz, v_Colour.w), mix(alpha, 0.0, progress));
    }
}