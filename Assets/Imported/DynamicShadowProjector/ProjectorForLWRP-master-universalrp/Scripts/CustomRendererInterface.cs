//
// CustomRendererInterface.cs
//
// Projector For LWRP
//
// Copyright (c) 2020 NYAHOON GAMES PTE. LTD.
//

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ProjectorForLWRP
{
    public interface ICustomRenderer
    {
        RenderPassEvent renderPassEvent { get; }
        void Render(ScriptableRenderContext context, ref RenderingData renderingData);
    }
}
