// this shader can be used to disallow drawing over a certain region of screen
// creating a "mask" or a "substract" union between 2 shapes
// to use it:
//  writeDepth = true;
//  GLWrapper.PushDepthInfo( new DepthInfo( depthTest: true, writeDepth: true, function: DepthFunction.Always ) );
//  DrawMaskShape();
//  GLWrapper.PopDepthInfo();
//  ... // draw with different shader
// after this step, you are left with the mask still there.
// to erase it:
//  writeDepth = false;
//  GLWrapper.PushDepthInfo( new DepthInfo( depthTest: true, writeDepth: true, function: DepthFunction.Always ) );
//  DrawMaskShape();
//  GLWrapper.PopDepthInfo();

layout(location = 0) out vec4 o_colour;

layout(std140, set = 0, binding = 0) uniform m_maskParameters {
    bool writeDepth;
};

void main(void) 
{
    o_colour = vec4(0.0, 0.0, 0.0, 0.0);

    if (writeDepth) {
        gl_FragDepth = 0.0;
    }
    else {
        gl_FragDepth = 1.0;
    }
}