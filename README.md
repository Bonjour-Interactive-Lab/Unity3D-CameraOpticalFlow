# Unity3D-CameraOpticalFlow
![gif](https://i.imgur.com/9V83DvQ.gif)

*CameraOpticalFlow* is a GPU based optical flow system for unity.

### OpticalFlow.cs
This script computes an optical flow on GPU side.
It takes a RenderTexture as an input to analyze.
The result is returned as a RenderTexture in an ARGBFloat format.
You can grab the RenderTexture containing the OF from script by calling ```OpticalFlow.GetOpticalFlowMap()```

### OFTrailSystemUpdater.cs
This script compute an history trail from the optical flow.
It grab the result from ```OpticalFlow.cs```
The results is return as a CustomRenderTexture in an ARGBFloat format.
You could get the result of the OF from script by calling ```OFTrailSystemUpdater.GetOFTrail()```

### AverageVelocity.cs
This script compute the average velocity from the OF texture.
Please note: this computation can be pretty intensive as it loop over all the pixel of the image. Use it with a lower resolution defined by the var ```resolution```
The result can be grab using ```AverageVelocity.GetAverageVelocity()```


Test on
-------
* Unity 2020 & 2019
