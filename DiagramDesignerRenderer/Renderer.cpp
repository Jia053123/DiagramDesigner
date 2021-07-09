#include "RendererHeader.h"

// handle window messages. For the WM_SIZE message, call the OnResize method and pass it the new width and height. 
// For the WM_PAINT and WM_DISPLAYCHANGE messages, call the OnRender method to paint the window. 
LRESULT CALLBACK DiagramDesignerRenderer::WndProc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    LRESULT result = 0;

    if (message == WM_CREATE)
    {
        LPCREATESTRUCT pcs = (LPCREATESTRUCT)lParam;
        DiagramDesignerRenderer* renderer = (DiagramDesignerRenderer*)pcs->lpCreateParams;

        ::SetWindowLongPtrW(
            hwnd,
            GWLP_USERDATA,
            reinterpret_cast<LONG_PTR>(renderer)
        );

        result = 1;
    }
    else
    {
        DiagramDesignerRenderer* renderer = reinterpret_cast<DiagramDesignerRenderer*>(static_cast<LONG_PTR>(
            ::GetWindowLongPtrW(
                hwnd,
                GWLP_USERDATA
            )));

        bool wasHandled = false;

        if (renderer)
        {
            switch (message)
            {
            case WM_SIZE:
            {
                UINT width = LOWORD(lParam);
                UINT height = HIWORD(lParam);
                renderer->OnResize(width, height);
            }
            result = 0;
            wasHandled = true;
            break;

            case WM_DISPLAYCHANGE:
            {
                InvalidateRect(hwnd, NULL, FALSE);
            }
            result = 0;
            wasHandled = true;
            break;

            case WM_PAINT:
            {
                renderer->OnRender();
                ValidateRect(hwnd, NULL);
            }
            result = 0;
            wasHandled = true;
            break;

            case WM_DESTROY:
            {
                PostQuitMessage(0);
            }
            result = 1;
            wasHandled = true;
            break;
            }
        }

        if (!wasHandled)
        {
            result = DefWindowProc(hwnd, message, wParam, lParam);
        }
    }

    return result;

}

HRESULT DiagramDesignerRenderer::OnRender()
{
    // First, create an HRESULT. Then call the CreateDeviceResource method. This method is called every time the window is painted. 
    // Recall that, in step 4 of Part 3, you added an if statement to prevent the method from doing any work if the render target already exists.

    HRESULT hr = S_OK;

    hr = CreateDeviceResources();

    // Verify that the CreateDeviceResource method succeeded.If it didn't, don't perform any drawing.
    if (SUCCEEDED(hr))
    {
        // initiate drawing by calling the render target's BeginDraw method. Set the render target's transform to the identity matrix, and clear the window.
        m_pRenderTarget->BeginDraw();

        m_pRenderTarget->SetTransform(D2D1::Matrix3x2F::Identity());

        m_pRenderTarget->Clear(D2D1::ColorF(D2D1::ColorF::White));

        // Retrieve the size of the drawing area.
        D2D1_SIZE_F rtSize = m_pRenderTarget->GetSize();

        // Draw a grid background by using a for loop and the render target's DrawLine method to draw a series of lines.
        int width = static_cast<int>(rtSize.width);
        int height = static_cast<int>(rtSize.height);

        for (int x = 0; x < width; x += 10)
        {
            m_pRenderTarget->DrawLine(
                D2D1::Point2F(static_cast<FLOAT>(x), 0.0f),
                D2D1::Point2F(static_cast<FLOAT>(x), rtSize.height),
                m_pLightSlateGrayBrush,
                0.5f
            );
        }

        for (int y = 0; y < height; y += 10)
        {
            m_pRenderTarget->DrawLine(
                D2D1::Point2F(0.0f, static_cast<FLOAT>(y)),
                D2D1::Point2F(rtSize.width, static_cast<FLOAT>(y)),
                m_pLightSlateGrayBrush,
                0.5f
            );
        }

        // Create two rectangle primitives that are centered on the screen.
        D2D1_RECT_F rectangle1 = D2D1::RectF(
            rtSize.width / 2 - 50.0f,
            rtSize.height / 2 - 50.0f,
            rtSize.width / 2 + 50.0f,
            rtSize.height / 2 + 50.0f
        );

        D2D1_RECT_F rectangle2 = D2D1::RectF(
            rtSize.width / 2 - 100.0f,
            rtSize.height / 2 - 100.0f,
            rtSize.width / 2 + 100.0f,
            rtSize.height / 2 + 100.0f
        );
        
        // Use the render target's FillRectangle method to paint the interior of the first rectangle with the gray brush.
        m_pRenderTarget->FillRectangle(&rectangle1, m_pLightSlateGrayBrush);

        // Use the render target's DrawRectangle method to paint the outline of the second rectangle with the cornflower blue brush.
        m_pRenderTarget->DrawRectangle(&rectangle2, m_pCornflowerBlueBrush);

        // Call the render target's EndDraw method. The EndDraw method returns an HRESULT to indicate whether the drawing operations were successful.
        hr = m_pRenderTarget->EndDraw();
    }
    
    // Check the HRESULT returned by EndDraw. 
    // If it indicates that the render target needs to be recreated, call the DiscardDeviceResources method to release it; 
    // it will be recreated the next time the window receives a WM_PAINT or WM_DISPLAYCHANGE message.
    if (hr == D2DERR_RECREATE_TARGET)
    {
        hr = S_OK;
        DiscardDeviceResources();
    }

    // Return the HRESULT
    return hr;
}

// resizes the render target to the new size of the window.
void DiagramDesignerRenderer::OnResize(UINT width, UINT height)
{
    if (m_pRenderTarget)
    {
        // Note: This method can fail, but it's okay to ignore the
        // error here, because the error will be returned again
        // the next time EndDraw is called.
        m_pRenderTarget->Resize(D2D1::SizeU(width, height));
    }
}
