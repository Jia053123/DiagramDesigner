#include "RendererHeader.h"

// create an ID2D1Factory, a device-independent resource, for creating other Direct2D resources. Use the m_pDirect2DdFactory class member to store the factory.
HRESULT DiagramDesignerRenderer::CreateDeviceIndependentResources()
{
    HRESULT hr = S_OK;

    // Create a Direct2D factory.
    hr = D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED, &m_pDirect2dFactory);

    return hr;
}

// This method creates the window's device-dependent resources, a render target, and two brushes.
// Retrieve the size of the client area and create an ID2D1HwndRenderTarget of the same size that renders to the window's HWND. 
// Store the render target in the m_pRenderTarget class member.
HRESULT DiagramDesignerRenderer::CreateDeviceResources()
{
    HRESULT hr = S_OK;

    // Because this method will be called repeatedly, add an if statement to check whether the render target (m_pRenderTarget ) already exists.
    if (!m_pRenderTarget)
    {
        RECT rc;
        GetClientRect(m_hwnd, &rc);

        D2D1_SIZE_U size = D2D1::SizeU(
            rc.right - rc.left,
            rc.bottom - rc.top
        );

        // Create a Direct2D render target.
        hr = m_pDirect2dFactory->CreateHwndRenderTarget(
            D2D1::RenderTargetProperties(),
            D2D1::HwndRenderTargetProperties(m_hwnd, size),
            &m_pRenderTarget
        );


        if (SUCCEEDED(hr))
        {
            // Create a gray brush.
            hr = m_pRenderTarget->CreateSolidColorBrush(
                D2D1::ColorF(D2D1::ColorF::LightSlateGray),
                &m_pLightSlateGrayBrush
            );
        }
        if (SUCCEEDED(hr))
        {
            // Create a blue brush.
            hr = m_pRenderTarget->CreateSolidColorBrush(
                D2D1::ColorF(D2D1::ColorF::CornflowerBlue),
                &m_pCornflowerBlueBrush
            );
        }
    }

    return hr;
}

void DiagramDesignerRenderer::DiscardDeviceResources()
{
    SafeRelease(&m_pRenderTarget);
    SafeRelease(&m_pLightSlateGrayBrush);
    SafeRelease(&m_pCornflowerBlueBrush);
}

