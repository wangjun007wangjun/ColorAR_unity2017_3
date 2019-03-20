// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Color/Special" {
//Shader的路径和名称
    Properties {
    //材质属性面板中所显示的Shader属性面板
        _MainTex ("Base (RGB)", 2D) = "white" {}
        //"_MainTex"在Shader中调用时所使用的名称
        //"Base (RGB)"在面板中显示的名称
        //"2D"2D纹理
        //"white"给这个属性的默认值

        //从C#中获取截图时 识别图四个点世界坐标
        _Uvpoint1("point1", Vector) = (0 , 0 , 0 , 0)
        //"_Uvpoint1"在Shader中调用时所使用的名称
        //"point1"在面板中所显示的名称
        //Vector 四个浮点数组成的类型 
        //"0 , 0 , 0 , 0"附的初始值
        _Uvpoint2("point2", Vector) = (0 , 0 , 0 , 0)
        _Uvpoint3("point3", Vector) = (0 , 0 , 0 , 0)
        _Uvpoint4("point4", Vector) = (0 , 0 , 0 , 0)

    }

    //“ SubShader”着色器方案 在Shader中至少有一个SubShader 显卡每次只选择一个SubShader 如果当前硬件不支持这个SubShader 就会选择一个针对较旧的硬件的SubShader
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        //加入透明渲染处理，没有这一段的话赋值透明贴图时就会出现问题。
        LOD 200
        //细致程度   Level of Details 也叫作 Level of Development
        //"200"是一个代号 限制Shader的级别到200为止

        Pass{
            Blend SrcAlpha OneMinusSrcAlpha
            //加入Alpha的混合渲染  不加的话Alpha值无用
            CGPROGRAM
            //CG开始的关键词
            #pragma vertex vert
            //编译指令 顶点程序
            #pragma fragment frag
            //编译指令 片段程序
            #include "UnityCG.cginc"
            //"UnityCG.cginc" 是使用unity中带的封装好的cg代码集合
            //有点类似于C#中命名空间的引用

            //C#中传递来的值的引用
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Uvpoint1;
            float4 _Uvpoint2;
            float4 _Uvpoint3;
            float4 _Uvpoint4;
			float4x4 _VP;
			//C#在截取图像时 世界坐标到摄像机坐标以及相机坐标到屏幕坐标的两个矩阵值相乘

			//结构体 
            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
                float4  fixedPos : TEXCOORD2;
            } ;

            //顶点程序和片段程序中用来计算UV的匹配和最护模型效果的渲染
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
				
                float4 top = lerp(_Uvpoint1, _Uvpoint3, o.uv.x);
                float4 bottom = lerp(_Uvpoint2, _Uvpoint4, o.uv.x);
                float4 fixedPos = lerp(bottom, top, o.uv.y);
                o.fixedPos = ComputeScreenPos(mul(UNITY_MATRIX_VP, fixedPos));
                return o;
            }
            float4 frag (v2f i) : COLOR
            {
			    
			    float4 top = lerp(_Uvpoint1, _Uvpoint3, i.uv.x);
                float4 bottom = lerp(_Uvpoint2, _Uvpoint4, i.uv.x);
                float4 fixedPos = lerp(bottom, top, i.uv.y);
				fixedPos = ComputeScreenPos(mul(_VP, fixedPos));
                return tex2D(_MainTex, fixedPos.xy / fixedPos.w);
				
            }
            ENDCG
            //CG结束的关键词
        }
    }
    
}