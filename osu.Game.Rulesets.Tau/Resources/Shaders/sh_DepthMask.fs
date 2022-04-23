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

uniform bool writeDepth;

void main(void) 
{
    gl_FragColor = vec4(0.0, 0.0, 0.0, 0.0);

    if (writeDepth) {
        gl_FragDepth = 0.0;
    }
    else {
        gl_FragDepth = 1.0;
    }
}