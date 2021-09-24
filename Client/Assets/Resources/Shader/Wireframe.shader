﻿Shader "XShader/Wireframe" 
{
        Properties
        {
            _Color( "Color",Color ) = ( 1,1,1,0 )
            _EdgeColor( "Edge Color",Color ) = ( 1,0.5,1,1 )
            _Width( "Width",Range( 0,1 ) ) = 0.01
        }

        SubShader
        {
            Tags 
            {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest Off
            Lighting Off
            Cull Off

            Pass 
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct a2v 
                {
                    half4 uv : TEXCOORD0;
                    half4 vertex : POSITION;
                };

                struct v2f 
                {
                    half4 pos : SV_POSITION;
                    half4 uv : TEXCOORD0;
                };

                fixed4 _Color;
                fixed4 _EdgeColor;
                float _Width;

                v2f vert( a2v v )
                {
                    v2f o;
                    o.uv = v.uv;
                    o.pos = UnityObjectToClipPos( v.vertex );
                    return o;
                }

                fixed4 frag( v2f i ) : COLOR
                {
                    fixed4 col;
                    float lx = step( _Width, i.uv.x );
                    float ly = step( _Width, i.uv.y );
                    float hx = step( i.uv.x, 1.0 - _Width );
                    float hy = step( i.uv.y, 1.0 - _Width );
                    col = lerp( _EdgeColor, _Color, lx * ly * hx * hy );
                    return col;
                }
                ENDCG
            }

        }
        FallBack "Diffuse"
}