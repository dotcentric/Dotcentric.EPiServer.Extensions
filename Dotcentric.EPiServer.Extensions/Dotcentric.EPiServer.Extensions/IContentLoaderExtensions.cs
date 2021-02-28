using EPiServer;
using EPiServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dotcentric.EPiServer.Extensions
{
    public static class IContentLoaderExtensions
    {
        //max number of levels in the content tree
        public const int defaultMaxLevel = 10;

        public static T GetAncestorOrSelf<T>(this IContentLoader contentLoader,
            IContent content,
            Func<T, bool> predicate = null,
            int maxLevel = defaultMaxLevel) where T : IContent
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var contentPivot = content;

            for (var i = 0; i < maxLevel; i++)
            {
                if (contentPivot is T typedContentPivot) 
                {
                    if (predicate == null)
                        return typedContentPivot;

                    else if (predicate.Invoke(typedContentPivot))
                        return typedContentPivot;
                }

                var parent = ContentReference.IsNullOrEmpty(contentPivot.ParentLink) ?
                null :
                contentLoader.Get<IContent>(contentPivot.ParentLink);

                //if the parent is null - end of navigation
                if (parent == null)
                    return default;

                //we are still inside the tree, we continue to loop
                contentPivot = parent;
            }

            //unlikely to happen but there to avoid stackoverflow exception. we return null
            return default;
        }

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
            return GetAncestorOrSelf<IContent>(contentLoader, content, predicate, maxLevel);
        }

        /// Same function than GetAncestorOrSelf but with the parent as part of a predicate
        /// so we dont need to call .Get<> to get the parent object twice
        /// </summary>
        /// <param name="contentLoader"></param>
        /// <param name="content"></param>
        /// <param name="predicate"></param>
        /// <param name="maxLevel"></param>
        /// <returns></returns>
        public static T GetAncestorOrSelf<T>(this IContentLoader contentLoader,
            IContent content, 
            Func<T, bool> predicate,
            Func<IContent, bool> parentPredicate,
            int maxLevel = defaultMaxLevel) where T : IContent
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            //if you want to use the function without predicate see GetAncestorOrSelf with the nullable predicate
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (parentPredicate == null)
                throw new ArgumentNullException(nameof(parentPredicate));

            //pivot will be the variable that will be used to navigate up the content tree
            var contentPivot = content;

            //please do not use 'while' loops
            for (var i = 0; i < maxLevel; i++)
            {
                var parent = ContentReference.IsNullOrEmpty(contentPivot.ParentLink) ?
                null :
                contentLoader.Get<IContent>(contentPivot.ParentLink);

                //we can allow the predicate to work even with a null parent
                if (contentPivot is T typedContentPivot && 
                    predicate.Invoke(typedContentPivot) && 
                    parentPredicate.Invoke(parent))
                    return typedContentPivot;

                //if the parent is null - end of navigation
                if (parent == null)
                    return default(T);

                //we are still inside the tree, we continue to loop
                contentPivot = parent;
            }

            //unlikely to happen but there to avoid stackoverflow exception. we return null
            return default(T);
        }

        public static IContent GetAncestorOrSelf(this IContentLoader contentLoader,
            IContent content,
            Func<IContent, bool> predicate,
            Func<IContent, bool> parentPredicate,
            int maxLevel = defaultMaxLevel) 
        {
            return GetAncestorOrSelf<IContent>(contentLoader, content, predicate, parentPredicate, maxLevel);
        }

        public static T GetAncestor<T>(this IContentLoader contentLoader,
            IContent content,
            Func<T, bool> predicate,
            int maxLevel = defaultMaxLevel) where T : IContent
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var parent = ContentReference.IsNullOrEmpty(content.ParentLink) ?
                null :
                contentLoader.Get<IContent>(content.ParentLink);

            return GetAncestorOrSelf<T>(contentLoader, content, predicate, maxLevel);
        }

        ///for this function we move to the first parent then call GetAncestorOrSelf
        public static IContent GetAncestor(this IContentLoader contentLoader,
            IContent content,
            Func<IContent, bool> predicate,
            int maxLevel = defaultMaxLevel)
        {
            return GetAncestor<IContent>(contentLoader, content, predicate, maxLevel);
        }
    
        public static IEnumerable<T> GetAncestors<T>(this IContentLoader contentLoader,
            IContent content,
            Func<T, bool> predicate = null,
            int maxLevel = defaultMaxLevel) where T : IContent
        {
            //default ? we return an empty list
            if (content.Equals(default(T)))
                return Enumerable.Empty<T>();

            //pivot will be the variable that will be used to navigate up the content tree
            var contentPivot = content;

            var toReturn = new List<T>();

            //please do not use 'while' loops
            for (var i = 0; i < maxLevel; i++)
            {
                //we only get ancestors of a specific type - if the type is not right then we can skip
                if(contentPivot is T)
                {
                    if (predicate == null)
                        toReturn.Add((T)contentPivot);

                    else if (predicate.Invoke((T)contentPivot))
                        toReturn.Add((T)contentPivot);
                }

                IContent parent = ContentReference.IsNullOrEmpty(contentPivot.ParentLink) ?
                null :
                contentLoader.Get<IContent>(contentPivot.ParentLink);

                //if the parent is null - end of navigation
                if (parent==null)
                    break;

                //we are still inside the tree, we continue to loop
                contentPivot = parent;
            }

            //we return our list of ancestors based on the predicate
            return toReturn;
        }
    
        public static IEnumerable<IContent> GetAncestors(this IContentLoader contentLoader,
            IContent content, 
            Func<IContent,bool> predicate = null, 
            int maxLevel = defaultMaxLevel)
        {
            return GetAncestors<IContent>(contentLoader, content, predicate, maxLevel);
        }

        /// <summary>
        /// Non recursive function to retrieve the first descendant or self based on the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentLoader"></param>
        /// <param name="content"></param>
        /// <param name="predicate"></param>
        /// <param name="maxLevel"></param>
        /// <returns></returns>
        public static T GetDescendantOrSelf<T>(this IContentLoader contentLoader, 
            IContent content, 
            Func<T, bool> predicate,
            int maxLevel = defaultMaxLevel) where T : IContent
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (content is T typedContent && predicate.Invoke(typedContent))
                return (T)content;

            IEnumerable<IContent> pivot = new List<IContent>
            {
                content
            };

            for (var i = 0; i < maxLevel; i++)
            {
                var children = pivot.SelectMany(x => contentLoader.GetChildren<IContent>(x.ContentLink));

                if (!children.Any())
                    break;

                var res = children
                    .OfType<T>()
                    .FirstOrDefault(predicate);

                //we can return the object is the value is not default nor null
                if (res != null)
                    return res;

                //we move to the next level
                pivot = children;
            }

            //we couldnt find a predicate so we just return the default value for the return
            return default;
        }

        /// <summary>
        /// Non recursive non generic function to retrieve the first descendant or self 
        /// based on the predicate
        /// </summary>
        /// <param name="contentLoader"></param>
        /// <param name="content"></param>
        /// <param name="predicate"></param>
        /// <param name="maxLevel"></param>
        /// <returns></returns>
        public static IContent GetDescendantOrSelf(this IContentLoader contentLoader, 
            IContent content, 
            Func<IContent, bool> predicate,
            int maxLevel = defaultMaxLevel)
        {
            return GetDescendantOrSelf<IContent>(contentLoader, content, predicate, maxLevel);
        }

        public static T GetDescendant<T>(this IContentLoader contentLoader,
            IContent content,
            Func<T, bool> predicate,
            int maxLevel = defaultMaxLevel) where T : IContent
        {
            IEnumerable<IContent> pivot = new List<IContent>
            {
                content
            };

            for (var i = 0; i < maxLevel; i++)
            {
                var children = pivot.SelectMany(x => contentLoader.GetChildren<IContent>(x.ContentLink));

                if (!children.Any())
                    break;

                var res = children
                    .OfType<T>()
                    .FirstOrDefault(predicate);

                //we can return the object is the value is not default nor null
                if (res != null)
                    return res;

                //we move to the next level
                pivot = children;
            }

            //we couldnt find a predicate so we just return the default value for the return
            return default;
        }

        public static IContent GetDescendant(this IContentLoader contentLoader,
            IContent content,
            Func<IContent, bool> predicate,
            int maxLevel = defaultMaxLevel)
        {
            return GetDescendant<IContent>(contentLoader, content, predicate, maxLevel);
        }

        public static IEnumerable<T> GetDescendants<T>(this IContentLoader contentLoader, 
            IContent content,
            Func<T, bool> predicate = null,
            int maxLevel = defaultMaxLevel)
        {
            var toReturn = new List<T>();

            IEnumerable<IContent> pivot = new List<IContent>
            {
                content
            };

            for (var i = 0; i < maxLevel; i++)
            {
                var children = pivot.SelectMany(x => contentLoader.GetChildren<IContent>(x.ContentLink));

                if (!children.Any())
                    break;

                pivot = children;

                var res = predicate == null ? children.OfType<T>() : children.OfType<T>().Where(predicate);

                toReturn.AddRange(res);
            }

            return toReturn;
        }
    
        public static IEnumerable<IContent> GetDescendants(this IContentLoader contentLoader,
            IContent content,
            Func<IContent, bool> predicate = null,
            int maxLevel = defaultMaxLevel)
        {
            return GetDescendants<IContent>(contentLoader, content, predicate, maxLevel);
        }

        public static IEnumerable<T> SiblingsAndSelf<T>(this IContentLoader contentLoader,
            IContent content,
            Func<T, bool> predicate = null) where T : IContent
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            //no parent - mostly due to be at the top of the content tree - we return an empty list
            if (ContentReference.IsNullOrEmpty(content.ParentLink))
                return Enumerable.Empty<T>();

            var success = contentLoader.TryGet(content.ParentLink, out IContent parent);

            if (!success)
                return Enumerable.Empty<T>();

            var siblings = contentLoader
                .GetChildren<T>(parent.ContentLink);

            var toReturn = predicate == null ? siblings : siblings.Where(predicate);

            return toReturn;
        }

        public static IEnumerable<IContent> SiblingsAndSelf(this IContentLoader contentLoader,
            IContent content, 
            Func<IContent, bool> predicate = null)
        {
            return SiblingsAndSelf<IContent>(contentLoader, content, predicate);
        }

        public static IEnumerable<T> Siblings<T>(this IContentLoader contentLoader,
            IContent content,
            Func<T, bool> predicate = null) where T : IContent
        {
            var siblings = SiblingsAndSelf(contentLoader, content, predicate)
                .Where(x => x.ContentLink.ID != content.ContentLink.ID);

            //predicate was applied at the SiblingsAndSelf level
            return siblings;
        }

        public static IEnumerable<IContent> Siblings(this IContentLoader contentLoader, 
            IContent content, 
            Func<IContent, bool> predicate = null)
        {
            return Siblings<IContent>(contentLoader, content, predicate);
        }

        public static T FirstSibling<T>(this IContentLoader contentLoader,
           IContent content,
           Func<T, bool> predicate) where T : IContent
        {
            return Siblings<T>(contentLoader, content, predicate).FirstOrDefault();
        }

        public static IContent FirstSibling(this IContentLoader contentLoader, 
            IContent content, 
            Func<IContent, bool> predicate)
        {
            return FirstSibling<IContent>(contentLoader, content, predicate);
        }

        public static T FirstChild<T>(this IContentLoader contentLoader,
            ContentReference contentReference,
            Func<T, bool> predicate = null) where T : IContentData
        {
            var children = contentLoader.GetChildren<T>(contentReference);

            return predicate == null ? children.FirstOrDefault() : children.FirstOrDefault(predicate);
        }

        public static IContentData FirstChild(this IContentLoader contentLoader,
            ContentReference contentReference,
            Func<IContentData, bool> predicate = null)
        {
            return FirstChild<IContentData>(contentLoader, contentReference, predicate);
        }

        public static T LastChild<T>(this IContentLoader contentLoader,
           ContentReference contentReference,
           Func<T, bool> predicate = null) where T : IContentData
        {
            var children = contentLoader.GetChildren<T>(contentReference);

            return predicate == null ? children.LastOrDefault() : children.LastOrDefault(predicate);
        }

        public static IContentData LastChild(this IContentLoader contentLoader,
            ContentReference contentReference,
            Func<IContentData, bool> predicate = null)
        {
            return LastChild<IContentData>(contentLoader, contentReference, predicate);
        }

        public static IEnumerable<IContent> OrderedSiblingsAndSelf<TKey>(this IContentLoader contentLoader,
           IContent content,
           Func<IContent, TKey> keySelector)
        {
            var siblings = SiblingsAndSelf<IContent>(contentLoader, content, null);

            //we order the list of children
            return siblings.OrderBy(keySelector).ToList();
        }

        /// <summary>
        /// Provides the next siblings of type T 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentLoader"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static IEnumerable<T> FollowingSiblings<T, TKey>(this IContentLoader contentLoader,
            IContent content,
            Func<IContent, TKey> keySelector)
            where T : IContent
        {
            var orderedChildren = OrderedSiblingsAndSelf(contentLoader, content, keySelector).ToList();

            var position = orderedChildren.IndexOf(content);

            var rightList = orderedChildren.Skip(position + 1).Take(orderedChildren.Count - position - 1);

            return rightList.OfType<T>();
        }

        public static IEnumerable<IContent> FollowingSiblings<TKey>(this IContentLoader contentLoader,
            IContent content,
            Func<IContent, TKey> keySelector)
        {
            return FollowingSiblings<IContent, TKey>(contentLoader, content, keySelector);
        }

        public static T FollowingSibling<T, TKey>(this IContentLoader contentLoader,
            IContent content,
            Func<IContent, TKey> keySelector)
            where T : IContent
        {
            return FollowingSiblings<T, TKey>(contentLoader, content, keySelector).FirstOrDefault();
        }

        public static IContent FollowingSibling<TKey>(this IContentLoader contentLoader,
            IContent content,
            Func<IContent, TKey> keySelector)
        {
            return FollowingSibling<IContent, TKey>(contentLoader, content, keySelector);
        }

        /// <summary>
        /// Provides the previous siblings of type T 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentLoader"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static IEnumerable<T> PreviousSiblings<T, TKey>(this IContentLoader contentLoader,
            IContent content,
            Func<IContent, TKey> keySelector)
            where T : IContent
        {
            var orderedChildren = OrderedSiblingsAndSelf(contentLoader, content, keySelector).ToList();

            var position = orderedChildren.IndexOf(content);

            var rightList = orderedChildren.Take(position);

            return rightList.OfType<T>();
        }

        public static IEnumerable<IContent> PreviousSiblings<TKey>(this IContentLoader contentLoader,
            IContent content,
            Func<IContent, TKey> keySelector)
        {
            return PreviousSiblings<IContent, TKey>(contentLoader, content, keySelector);
        }

        public static T PreviousSibling<T, TKey>(this IContentLoader contentLoader,
            IContent content,
            Func<IContent, TKey> keySelector)
            where T : IContent
        {
            return PreviousSiblings<T, TKey>(contentLoader, content, keySelector).LastOrDefault();
        }

        public static IContent PreviousSibling<TKey>(this IContentLoader contentLoader,
            IContent content,
            Func<IContent, TKey> keySelector)
        {
            return PreviousSibling<IContent, TKey>(contentLoader, content, keySelector);
        }
    }
}
