﻿using MusicWebAppBackend.Infrastructure.Helpers;

namespace MusicWebAppBackend.Infrastructure.Models.Entites
{
    public abstract class ParentEntity
    {
        protected ParentEntity()
        {
            _id = UniqueIdentifier.New;
        }

        public string Id
        {
            get { return _id; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _id = UniqueIdentifier.New;
                else
                    _id = value;
            }
        }

        private string _id;

    }
}
