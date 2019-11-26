using System;
using System.Collections.Generic;
using System.Linq;

namespace CameraVision
{
    public class RecognizedTextItem
    {
        public string TopCandidate { get; set; }
        public string OtherCandidates { get; set; }

        public RecognizedTextItem(List<string> items)
        {
            TopCandidate = items.FirstOrDefault();

            items.RemoveAt(0);

            OtherCandidates = string.Join("\n", items);
        }
    }
}
