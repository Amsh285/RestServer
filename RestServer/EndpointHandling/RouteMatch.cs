﻿using RestServer.EndpointHandling.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RestServer.EndpointHandling
{
    public sealed class RouteMatch
    {
        public RouteActionMatch ActionMatch { get; }

        public string[] TemplatePathSegments { get; }
        
        public string MatchingPath { get; }

        public string[] MatchingPathSegments { 
            get 
            { 
                return MatchingPath
                    .Split("/")
                    .Skip(1)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToArray();
            } 
        }

        public string[] MatchingActionPathSegments
        {
            get
            {
                return MatchingPathSegments
                    .Skip(1)
                    .ToArray();
            }
        }

        public RouteMatch(RouteActionMatch actionMatch, string matchingPath)
        {
            TemplatePathSegments = actionMatch.HttpMethod.GetTemplatePathSegments();
            ActionMatch = actionMatch;
            MatchingPath = matchingPath ?? throw new ArgumentNullException(nameof(matchingPath));
        }
    }
}
