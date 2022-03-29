#include "sh_Flashlight.h"

#define PI 3.14159265359
#define TWO_PI 6.28318530718

// highp precision is necessary for vertex positions to prevent catastrophic failure on GL_ES platforms
lowp vec4 getColourAt(highp vec2 diff, highp vec2 size, lowp vec4 originalColour)
{
    //highp float dist = length(diff);
    //highp float flashlightRadius = length(size);

    //return vec4(1.0, 1.0, 1.0, 1.0) * 
    //   vec4(1.0, 1.0, 1.0, smoothstep(flashlightRadius, flashlightRadius * smoothness, dist));

    // return vec4(diff, size.y, 1.0);

    var decomposed

    return vec4(v_Position / 300, diff.y + size.y / 100000, 1.0);
}