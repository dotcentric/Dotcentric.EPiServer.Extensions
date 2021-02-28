using EPiServer;
using EPiServer.Core;
using System;

namespace Dotcentric.EPiServer.Extensions
{
    public static class IContentLoaderExtensions
    {
        //max number of levels in the content tree
        public const int defaultMaxLevel = 10;

        /// <summary>
        /// Get the first ancestor based on the predicate - use cache functions
        /// </summary>
        /// <param name="contentLoader"></param>
        /// <param name="predicate">
        /// 1st parameter of the predicate is the content pivot, 
        /// 2nd is the predicate
        /// </param>
        /// <returns></returns>
        public static IContent GetAncestorOrSelf(this IContentLoader contentLoader,
            IContent content,
            Func<IContent, bool> predicate,
            int maxLevel = defaultMaxLevel)
        {
            if (content == null)
                return null;

            //pivot will be the variable that will be used to navigate up the content tree
            var contentPivot = content;

            //please do not use 'while' loops
            for (var i = 0; i < maxLevel; i++)
            {
                if (predicate.Invoke(contentPivot))
                    return contentPivot;

                var parent = ContentReference.IsNullOrEmpty(contentPivot.ParentLink) ?
                null :
                contentLoader.Get<IContent>(contentPivot.ParentLink);

                //if the parent is null - end of navigation
                if (parent == null)
                    return null;

                //we are still inside the tree, we continue to loop
                contentPivot = parent;
            }

            //unlikely to happen but there to avoid stackoverflow exception. we return null
            return null;
        }

        /// Same function than GetAncestorOrSelf but with the parent as part of the predicate
        /// so we dont need to call .Get<> to get the parent object twice
        /// </summary>
        /// <param name="contentLoader"></param>
        /// <param name="content"></param>
        /// <param name="predicate"></param>
        /// <param name="maxLevel"></param>
        /// <returns></returns>
        public static IContent GetAncestorOrSelf(this IContentLoader contentLoader,
           IContent content,
           Func<IContent, IContent, bool> predicate,
           int maxLevel = defaultMaxLevel)
        {
            if (content == null)
                return null;

            //pivot will be the variable that will be used to navigate up the content tree
            var contentPivot = content;

            //please do not use 'while' loops
            for (var i = 0; i < maxLevel; i++)
            {
                var parent = ContentReference.IsNullOrEmpty(contentPivot.ParentLink) ?
                null :
                contentLoader.Get<IContent>(contentPivot.ParentLink);

                //we can allow the predicate to work even with a null parent
                if (predicate.Invoke(contentPivot, parent))
                    return contentPivot;

                //if the parent is null - end of navigation
                if (parent == null)
                    return null;

                //we are still inside the tree, we continue to loop
                contentPivot = parent;
            }

            //unlikely to happen but there to avoid stackoverflow exception. we return null
            return null;
        }

        public static T GetAncestorOrSelf<T>(this IContentLoader contentLoader,
            IContent content,
            int maxLevel = defaultMaxLevel) where T : IContent
        {
            var res = contentLoader.GetAncestorOrSelf(content,
                (pivot) => pivot is T, maxLevel);

            if (res == null)
                return default(T);

            return (T)res;
        }

        public static T GetAncestorOrSelf<T>(this IContentLoader contentLoader,
            IContent content,
            Func<T, bool> predicate, 
            int maxLevel = defaultMaxLevel) where T : IContent
        {
            if (content == null)
                return default;

            //the new predicate is the predicate about the type + the predicate in param
            Func<T, bool> fullPredicate = (pivot) => pivot is T typedObj && predicate.Invoke(typedObj);

            return GetAncestorOrSelf(contentLoader, content, predicate, maxLevel);
        }
       

        public static T GetAncestor<T>(this IContentLoader contentLoader,
            IContent content,
            int maxLevel = defaultMaxLevel) where T : IContent
        {
            var res = contentLoader.GetAncestor(content,
                (pivot) => pivot is T, maxLevel);

            if (res == null)
                return default(T);

            return (T) res;
        }

        public static T GetAncestor<T>(this IContentLoader contentLoader,
          IContent content,
          Func<T, bool> predicate,
          int maxLevel = defaultMaxLevel) where T : IContent
        {
            var res = contentLoader.GetAncestor(content,
                (pivot) => pivot is T typedObj && predicate.Invoke(typedObj), maxLevel);

            if (res == null)
                return default(T);

            return (T) res;
        }

        ///for this function we move to the first parent then call GetAncestorOrSelf
        public static IContent GetAncestor(this IContentLoader contentLoader,
            IContent content,
            Func<IContent, bool> predicate,
            int maxLevel = defaultMaxLevel)
        {
            if (content == null)
                return null;

            var parent = ContentReference.IsNullOrEmpty(content.ParentLink) ?
                null :
                contentLoader.Get<IContent>(content.ParentLink);

            //pivot will be the variable that will be used to navigate up the content tree 
            //we start with the parent
            var contentPivot = parent;

            return GetAncestorOrSelf(contentLoader, contentPivot, predicate, maxLevel);
        }
    }
}
