﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ProjectOxford.Text.Core
{
    public abstract class TextRequest
    {
        public TextRequest()
        {
            this.Documents = new List<Document>();
        }

        [JsonProperty("documents")]
        public List<Document> Documents { get; set; }

        public void Validate()
        {
            //must have at least one document
            if(this.Documents == null || this.Documents.Count <=0)
                throw new DocumentCollectionMinDocumentException(0, 1);

            //must not have more than 1000 documents
            if (this.Documents.Count > 1000)
                throw new DocumentCollectionMaxDocumentException(this.Documents.Count, 1000);

            var collectionSize = 0;
            var documentIds = new List<string>();

            foreach(var document in this.Documents)
            {
                //document must have an id
                if(string.IsNullOrWhiteSpace(document.Id))
                    throw new DocumentIdRequiredException();

                //document id's must be unique
                if(documentIds.Contains(document.Id))
                    throw new DocumentCollectionDuplicateIdException(document.Id);
                else
                    documentIds.Add(document.Id);

                //document size must be greater than 0 and less than or equal to 10KB
                if (document.Size <= 0)
                    throw new DocumentMinSizeException(document.Id, document.Size, 1);

                if (document.Size > 10240)
                    throw new DocumentMaxSizeException(document.Id, document.Size, 10240);

                collectionSize = collectionSize + document.Size;
            }

            //total size of all documents cannot exceed 1MB
            if(collectionSize > 1048576)
                throw new DocumentCollectionMaxSizeException(collectionSize, 1048576);
        }
    }
}
