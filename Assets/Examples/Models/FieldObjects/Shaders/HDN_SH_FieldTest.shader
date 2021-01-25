// Shader created with Shader Forge v1.42 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Enhanced by Antoine Guillon / Arkham Development - http://www.arkham-development.com/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.42;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33392,y:32729,varname:node_3138,prsc:2|emission-2289-OUT,clip-584-OUT,voffset-2249-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:31993,y:32275,ptovrint:False,ptlb:Color_Blue,ptin:_Color_Blue,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Color,id:3505,x:31983,y:32606,ptovrint:False,ptlb:Color_Red,ptin:_Color_Red,varname:_Color_Blue_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Color,id:3630,x:32223,y:32811,ptovrint:False,ptlb:Color_White,ptin:_Color_White,varname:_Color_Red_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_VertexColor,id:3132,x:33604,y:32825,varname:node_3132,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4110,x:32537,y:32797,varname:node_4110,prsc:2|A-1765-OUT,B-3630-RGB;n:type:ShaderForge.SFN_Multiply,id:8283,x:32470,y:32078,varname:node_8283,prsc:2|A-6347-OUT,B-7241-RGB;n:type:ShaderForge.SFN_Multiply,id:3587,x:32470,y:32255,varname:node_3587,prsc:2|A-3801-OUT,B-3505-RGB;n:type:ShaderForge.SFN_Add,id:2289,x:32894,y:32535,varname:node_2289,prsc:2|A-1014-OUT,B-4110-OUT;n:type:ShaderForge.SFN_RemapRange,id:361,x:33922,y:32612,varname:node_361,prsc:2,frmn:0,frmx:1,tomn:0.5,tomx:-0.5|IN-3132-R;n:type:ShaderForge.SFN_Subtract,id:1217,x:33812,y:32968,varname:node_1217,prsc:2|A-3132-A,B-2824-OUT;n:type:ShaderForge.SFN_Vector1,id:2824,x:33812,y:33086,varname:node_2824,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Ceil,id:7189,x:33981,y:32968,varname:node_7189,prsc:2|IN-1217-OUT;n:type:ShaderForge.SFN_Set,id:9890,x:34226,y:32968,varname:Mask_Blue,prsc:2|IN-7189-OUT;n:type:ShaderForge.SFN_Subtract,id:3236,x:33812,y:33139,varname:node_3236,prsc:2|A-3132-A,B-7004-OUT;n:type:ShaderForge.SFN_Vector1,id:7004,x:33812,y:33257,varname:node_7004,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Ceil,id:6514,x:33981,y:33139,varname:node_6514,prsc:2|IN-3236-OUT;n:type:ShaderForge.SFN_Subtract,id:4914,x:34140,y:33139,varname:node_4914,prsc:2|A-6514-OUT,B-7189-OUT;n:type:ShaderForge.SFN_Set,id:4056,x:34316,y:33175,varname:Mask_Red,prsc:2|IN-4914-OUT;n:type:ShaderForge.SFN_Ceil,id:118,x:33981,y:33336,varname:node_118,prsc:2|IN-8647-OUT;n:type:ShaderForge.SFN_OneMinus,id:6050,x:34190,y:33320,varname:node_6050,prsc:2|IN-118-OUT;n:type:ShaderForge.SFN_Set,id:9893,x:34302,y:33320,varname:Mask_White,prsc:2|IN-6050-OUT;n:type:ShaderForge.SFN_RemapRange,id:3303,x:33922,y:32793,varname:node_3303,prsc:2,frmn:0,frmx:1,tomn:-0.5,tomx:0.5|IN-3132-B;n:type:ShaderForge.SFN_Set,id:1259,x:34087,y:32612,varname:Offset_X,prsc:2|IN-361-OUT;n:type:ShaderForge.SFN_Set,id:4105,x:34087,y:32705,varname:Offset_Y,prsc:2|IN-3132-G;n:type:ShaderForge.SFN_Set,id:4466,x:34087,y:32793,varname:Offset_Z,prsc:2|IN-3303-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3835,x:32505,y:33539,ptovrint:False,ptlb:Value_Width,ptin:_Value_Width,varname:node_3835,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:6;n:type:ShaderForge.SFN_ValueProperty,id:1110,x:32505,y:33689,ptovrint:False,ptlb:Value_Depth,ptin:_Value_Depth,varname:_Value_Width_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_ValueProperty,id:4133,x:32505,y:33612,ptovrint:False,ptlb:Value_Hight,ptin:_Value_Hight,varname:_Value_Depth_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2.66;n:type:ShaderForge.SFN_Multiply,id:8717,x:32692,y:33465,varname:node_8717,prsc:2|A-3835-OUT,B-8002-OUT;n:type:ShaderForge.SFN_Get,id:8002,x:32671,y:33587,varname:node_8002,prsc:2|IN-1259-OUT;n:type:ShaderForge.SFN_Multiply,id:4599,x:32692,y:33638,varname:node_4599,prsc:2|A-4133-OUT,B-8771-OUT;n:type:ShaderForge.SFN_Get,id:8771,x:32671,y:33760,varname:node_8771,prsc:2|IN-4105-OUT;n:type:ShaderForge.SFN_Multiply,id:2583,x:32692,y:33810,varname:node_2583,prsc:2|A-1110-OUT,B-212-OUT;n:type:ShaderForge.SFN_Get,id:212,x:32671,y:33932,varname:node_212,prsc:2|IN-4466-OUT;n:type:ShaderForge.SFN_Append,id:2249,x:33102,y:33629,varname:node_2249,prsc:2|A-5260-OUT,B-2970-OUT,C-970-OUT;n:type:ShaderForge.SFN_Get,id:6347,x:32449,y:32031,varname:node_6347,prsc:2|IN-9890-OUT;n:type:ShaderForge.SFN_Get,id:3801,x:32449,y:32211,varname:node_3801,prsc:2|IN-4056-OUT;n:type:ShaderForge.SFN_Get,id:1765,x:32517,y:32753,varname:node_1765,prsc:2|IN-9893-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:1014,x:32852,y:32314,ptovrint:False,ptlb:Switch_ColorInvert,ptin:_Switch_ColorInvert,varname:node_1014,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-6381-OUT,B-9918-OUT;n:type:ShaderForge.SFN_Multiply,id:2321,x:32473,y:32416,varname:node_2321,prsc:2|A-4613-OUT,B-3505-RGB;n:type:ShaderForge.SFN_Multiply,id:9052,x:32473,y:32593,varname:node_9052,prsc:2|A-7132-OUT,B-7241-RGB;n:type:ShaderForge.SFN_Get,id:4613,x:32452,y:32369,varname:node_4613,prsc:2|IN-9890-OUT;n:type:ShaderForge.SFN_Get,id:7132,x:32452,y:32549,varname:node_7132,prsc:2|IN-4056-OUT;n:type:ShaderForge.SFN_Add,id:6381,x:32668,y:32314,varname:node_6381,prsc:2|A-8283-OUT,B-3587-OUT;n:type:ShaderForge.SFN_Add,id:9918,x:32668,y:32453,varname:node_9918,prsc:2|A-2321-OUT,B-9052-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:5389,x:32685,y:33024,ptovrint:False,ptlb:Show_Red,ptin:_Show_Red,varname:node_5389,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-6420-OUT,B-9337-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:6757,x:32685,y:33171,ptovrint:False,ptlb:Show_Blue,ptin:_Show_Blue,varname:_Show_REd_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-6420-OUT,B-3586-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:6464,x:32685,y:33320,ptovrint:False,ptlb:Show_White,ptin:_Show_White,varname:_Show_Blue_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-6420-OUT,B-6276-OUT;n:type:ShaderForge.SFN_Get,id:3586,x:32439,y:33204,varname:node_3586,prsc:2|IN-9890-OUT;n:type:ShaderForge.SFN_Get,id:9337,x:32458,y:33047,varname:node_9337,prsc:2|IN-4056-OUT;n:type:ShaderForge.SFN_Get,id:6276,x:32439,y:33316,varname:node_6276,prsc:2|IN-9893-OUT;n:type:ShaderForge.SFN_Vector1,id:6420,x:32525,y:33132,varname:node_6420,prsc:2,v1:0;n:type:ShaderForge.SFN_Add,id:584,x:32847,y:33113,varname:node_584,prsc:2|A-5389-OUT,B-6757-OUT,C-6464-OUT;n:type:ShaderForge.SFN_Slider,id:6101,x:31705,y:33614,ptovrint:False,ptlb:Value_LineTickness,ptin:_Value_LineTickness,varname:node_6101,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_ComponentMask,id:8797,x:32379,y:33788,varname:node_8797,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-1041-OUT;n:type:ShaderForge.SFN_Multiply,id:1041,x:32212,y:33788,varname:node_1041,prsc:2|A-1191-OUT,B-9490-OUT;n:type:ShaderForge.SFN_RemapRange,id:9490,x:32036,y:33614,varname:node_9490,prsc:2,frmn:0,frmx:1,tomn:-0.024,tomx:0.1|IN-6101-OUT;n:type:ShaderForge.SFN_Add,id:5260,x:32881,y:33465,varname:node_5260,prsc:2|A-8717-OUT,B-8797-R;n:type:ShaderForge.SFN_Add,id:2970,x:32881,y:33629,varname:node_2970,prsc:2|A-4599-OUT,B-8797-G;n:type:ShaderForge.SFN_Add,id:970,x:32881,y:33810,varname:node_970,prsc:2|A-2583-OUT,B-8797-B;n:type:ShaderForge.SFN_NormalVector,id:1191,x:32036,y:33788,prsc:2,pt:False;n:type:ShaderForge.SFN_Subtract,id:8647,x:33812,y:33312,varname:node_8647,prsc:2|A-3132-A,B-7249-OUT;n:type:ShaderForge.SFN_Vector1,id:7249,x:33812,y:33430,varname:node_7249,prsc:2,v1:0.1;proporder:7241-3505-3630-3835-1110-4133-1014-5389-6757-6464-6101;pass:END;sub:END;*/

Shader "Shader Forge/HDN_SH_FieldTest" {
    Properties {
        _Color_Blue ("Color_Blue", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Color_Red ("Color_Red", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Color_White ("Color_White", Color) = (0.07843138,0.3921569,0.7843137,1)
        _Value_Width ("Value_Width", Float ) = 6
        _Value_Depth ("Value_Depth", Float ) = 10
        _Value_Hight ("Value_Hight", Float ) = 2.66
        [MaterialToggle] _Switch_ColorInvert ("Switch_ColorInvert", Float ) = 0
        [MaterialToggle] _Show_Red ("Show_Red", Float ) = 0
        [MaterialToggle] _Show_Blue ("Show_Blue", Float ) = 0
        [MaterialToggle] _Show_White ("Show_White", Float ) = 0
        _Value_LineTickness ("Value_LineTickness", Range(0, 1)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #ifndef UNITY_PASS_FORWARDBASE
            #define UNITY_PASS_FORWARDBASE
            #endif //UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore metal 
            #pragma target 3.0
            uniform float4 _Color_Blue;
            uniform float4 _Color_Red;
            uniform float4 _Color_White;
            uniform float _Value_Width;
            uniform float _Value_Depth;
            uniform float _Value_Hight;
            uniform fixed _Switch_ColorInvert;
            uniform fixed _Show_Red;
            uniform fixed _Show_Blue;
            uniform fixed _Show_White;
            uniform float _Value_LineTickness;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float Offset_X = (o.vertexColor.r*-1.0+0.5);
                float3 node_8797 = (v.normal*(_Value_LineTickness*0.124+-0.024)).rgb;
                float Offset_Y = o.vertexColor.g;
                float Offset_Z = (o.vertexColor.b*1.0+-0.5);
                v.vertex.xyz += float3(((_Value_Width*Offset_X)+node_8797.r),((_Value_Hight*Offset_Y)+node_8797.g),((_Value_Depth*Offset_Z)+node_8797.b));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float node_6420 = 0.0;
                float node_7189 = ceil((i.vertexColor.a-0.7));
                float Mask_Red = (ceil((i.vertexColor.a-0.3))-node_7189);
                float Mask_Blue = node_7189;
                float Mask_White = (1.0 - ceil((i.vertexColor.a-0.1)));
                float node_584 = (lerp( node_6420, Mask_Red, _Show_Red )+lerp( node_6420, Mask_Blue, _Show_Blue )+lerp( node_6420, Mask_White, _Show_White ));
                clip(node_584 - 0.5);
////// Lighting:
////// Emissive:
                float3 node_2289 = (lerp( ((Mask_Blue*_Color_Blue.rgb)+(Mask_Red*_Color_Red.rgb)), ((Mask_Blue*_Color_Red.rgb)+(Mask_Red*_Color_Blue.rgb)), _Switch_ColorInvert )+(Mask_White*_Color_White.rgb));
                float3 emissive = node_2289;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #ifndef UNITY_PASS_SHADOWCASTER
            #define UNITY_PASS_SHADOWCASTER
            #endif //UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore metal 
            #pragma target 3.0
            uniform float _Value_Width;
            uniform float _Value_Depth;
            uniform float _Value_Hight;
            uniform fixed _Show_Red;
            uniform fixed _Show_Blue;
            uniform fixed _Show_White;
            uniform float _Value_LineTickness;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float Offset_X = (o.vertexColor.r*-1.0+0.5);
                float3 node_8797 = (v.normal*(_Value_LineTickness*0.124+-0.024)).rgb;
                float Offset_Y = o.vertexColor.g;
                float Offset_Z = (o.vertexColor.b*1.0+-0.5);
                v.vertex.xyz += float3(((_Value_Width*Offset_X)+node_8797.r),((_Value_Hight*Offset_Y)+node_8797.g),((_Value_Depth*Offset_Z)+node_8797.b));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float node_6420 = 0.0;
                float node_7189 = ceil((i.vertexColor.a-0.7));
                float Mask_Red = (ceil((i.vertexColor.a-0.3))-node_7189);
                float Mask_Blue = node_7189;
                float Mask_White = (1.0 - ceil((i.vertexColor.a-0.1)));
                float node_584 = (lerp( node_6420, Mask_Red, _Show_Red )+lerp( node_6420, Mask_Blue, _Show_Blue )+lerp( node_6420, Mask_White, _Show_White ));
                clip(node_584 - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
