﻿

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record MergeClassModel
    {
        public Guid SourceClassId { get; init; }
        public Guid TargetClassId { get; init; }
    }
}
