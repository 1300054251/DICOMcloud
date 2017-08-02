﻿
using DICOMcloud.Wado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace DICOMcloud.Wado
{
    public class QidoRequestModelConverter 
    {
        public QidoRequestModelConverter ( )
        { }

        public bool TryParse ( HttpRequestMessage request, out IQidoRequestModel result )
        {
            QidoRequestModel wadoReq = new QidoRequestModel ( ) ;
            
            wadoReq.Query = new QidoQuery ( ) ;

            wadoReq.AcceptHeader        = request.Headers.Accept;
            wadoReq.AcceptCharsetHeader = request.Headers.AcceptCharset;

            var query = request.RequestUri.ParseQueryString ( ) ;
         
            foreach ( var key in query )
            {
                string queryKey = ((string)key).Trim ( ).ToLower ( ) ;

                switch ( queryKey )
                {
                    case QidoRequestKeys.FuzzyMatching:
                    {
                        bool fuzzy ;
                        
                        if ( bool.TryParse ( query[QidoRequestKeys.FuzzyMatching], out fuzzy ) )
                        {
                            wadoReq.FuzzyMatching = fuzzy ;
                        }
                    }
                    break ;

                    case QidoRequestKeys.Limit:
                    {
                        int limit ;

                        if ( int.TryParse ( query[QidoRequestKeys.Limit], out limit ) )
                        {
                            wadoReq.Limit = limit ;
                        }
                    }
                    break ;

                    case QidoRequestKeys.Offset:
                    {
                        int offset;
        
                        if ( int.TryParse ( query[QidoRequestKeys.Offset], out offset ) )
                        {
                            wadoReq.Offset = offset ;
                        }
                    }
                    break ;

                    case QidoRequestKeys.IncludeField:
                    {
                        string includeFields = query[QidoRequestKeys.IncludeField] ;

                        if ( !string.IsNullOrWhiteSpace (includeFields))
                        {
                            wadoReq.Query.IncludeElements.AddRange ( includeFields.Split (new char[] {','}, StringSplitOptions.RemoveEmptyEntries)) ;
                        }
                    }
                    break ;

                    default:
                    {
                        string   matchingQuery       = query[queryKey].Trim ( ) ;
                        //string[] matchingQueryValues = matchingQuery.Trim ( ).Split ( new char[] {','}, StringSplitOptions.RemoveEmptyEntries ) ;
                        var      matchingElements    = wadoReq.Query.MatchingElements ;
                        
                        //TODO: what if key exists   
                        //should return invalid request                      
                        //foreach ( var matchingParam in matchingQueryValues )
                        {
                            matchingElements.Add ( queryKey, matchingQuery) ;
                        }
                    }
                    break ;

                }
            }

            result = wadoReq ;

            return true ;
      }

      private WadoBurnAnnotation ParseAnnotation ( string annotationString)
      {
         WadoBurnAnnotation annotation = WadoBurnAnnotation.None ;

         if ( !string.IsNullOrWhiteSpace ( annotationString ) )
         { 
            string[] parts = annotationString.Trim().Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries) ;
         
            foreach(string part in parts)
            {
               WadoBurnAnnotation tempAnn ; 
               
               if ( Enum.TryParse<WadoBurnAnnotation>(part.Trim(), true, out tempAnn ) )
               { 
                  annotation |= tempAnn ;
               }
            }
         }

         return annotation ;
      }

      private int? GetIntValue ( string stringValue )
      {
         if ( string.IsNullOrWhiteSpace(stringValue))
         {
            return null ;
         }
         else
         { 
            int parsedVal ;

            if ( int.TryParse (stringValue.Trim(), out parsedVal))
            { 
               return parsedVal ;
            }
            else
            { 
               return null ;
            }
         }
      }
   }

   public abstract class QidoRequestKeys
   {
      private QidoRequestKeys (){} 

      public const string FuzzyMatching = "fuzzymatching" ;
      public const string Limit         = "limit" ;
      public const string Offset        = "offset" ;
      public const string IncludeField  = "includefield" ;
   }
}
