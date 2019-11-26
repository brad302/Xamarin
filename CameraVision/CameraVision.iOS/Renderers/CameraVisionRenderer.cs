using System;
using System.Collections.Generic;

using Foundation;
using Vision;
using VisionKit;
using CoreFoundation;

using CameraVision;
using CameraVision.iOS.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CameraVisionPage), typeof(CameraVisionRenderer))]
namespace CameraVision.iOS.Renderers
{
    public class CameraVisionRenderer : PageRenderer, IVNDocumentCameraViewControllerDelegate
    {
        CameraVisionPage _page;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            _page = Element as CameraVisionPage;

            _page.ShowDocumentViewController += ShowDocumentViewController;
        }

        public void ShowDocumentViewController()
        {
            VNDocumentCameraViewController cameraViewController;
            cameraViewController = new VNDocumentCameraViewController
            {
                Delegate = this
            };

            PresentViewController(cameraViewController, true, null);
        }

        [Export("documentCameraViewControllerDidCancel:")]
        public void DidCancel(VNDocumentCameraViewController controller)
        {
            DismissViewController(true, null);
        }

        [Export("documentCameraViewController:didFinishWithScan:")]
        public void DidFinish(VNDocumentCameraViewController controller, VNDocumentCameraScan scan)
        {
            var pageCount = (int)scan.PageCount;
            var allItems = new List<List<string>>();

            for (int i = 0; i < pageCount; i++)
            {
                var image = scan.GetImage(nuint.Parse(i.ToString()));
                var imageRequestHandler = new VNImageRequestHandler(image.CGImage, options: new NSDictionary());

                var textRequest = new VNRecognizeTextRequest(new VNRequestCompletionHandler((request, error) =>
                {
                    var results = request.GetResults<VNRecognizedTextObservation>();

                    foreach (var result in results)
                    {
                        var items = new List<string>();

                        foreach (var candidate in result.TopCandidates(100))
                            items.Add(candidate.String);

                        allItems.Add(items);
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        _page.LoadRecognizedTextItems(allItems);
                        DismissViewController(true, null);
                    });
                }));

                switch (_page.TextRecognitionLevel)
                {
                    case TextRecognitionLevelEnum.Accurate:
                        textRequest.RecognitionLevel = VNRequestTextRecognitionLevel.Accurate;
                        break;

                    case TextRecognitionLevelEnum.Fast:
                        textRequest.RecognitionLevel = VNRequestTextRecognitionLevel.Fast;
                        break;

                    default:
                        break;
                }

                textRequest.UsesLanguageCorrection = true;

                DispatchQueue.DefaultGlobalQueue.DispatchAsync(() =>
                {
                    imageRequestHandler.Perform(new VNRequest[] { textRequest }, out NSError error);
                });
            }
        }
    }
}
