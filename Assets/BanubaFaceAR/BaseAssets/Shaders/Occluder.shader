Shader "Unlit/Occluder"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
        SubShader {
        Tags { "Queue" = "Geometry-1" }
        ColorMask 0 
        ZWrite On
        Pass { }
    }
}
