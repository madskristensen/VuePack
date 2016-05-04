using System.Collections.Generic;
using Microsoft.Html.Editor.Completion;
using Microsoft.Html.Editor.Completion.Def;
using Microsoft.VisualStudio.Utilities;

namespace VuePack
{
    [HtmlCompletionProvider(CompletionTypes.Attributes, "*")]
    [ContentType("htmlx")]
    class AttributeCompletion : BaseCompletion
    {
        Dictionary<string, string> _attributes = new Dictionary<string, string>
        {
            { "track-by", "" },
            { "v-attr", "Updates the element’s given attribute (indicated by the argument)." },
            { "v-class", "If no argument is provided, it will add the binding’s value to the element’s classList, and update the class as the value changes." },
            { "v-cloak", "This property remains on the element until the associated ViewModel finishes compilation. Combined with CSS rules such as [v-cloak] { display: none }, this directive can be used to hide un-compiled mustache bindings until the ViewModel is ready." },
            { "v-el", "Register a reference to a DOM element on its owner Vue instance for easier access. e.g. <div v-el=\"hi\"> will be accessible as vm.$$.hi." },
            { "v-for", "We can use the v-for directive to render a list of items based on an Array. The v-for directive requires a special syntax in the form of item in items, where items is the source data Array and item is an alias for the Array element being iterated on" },
            { "v-html", "Updates the element’s innerHTML." },
            { "v-if", "Conditionally insert / remove the element based on the truthy-ness of the binding value. If the element is a <template> element, its content will be extracted as the conditional block." },
            { "v-model", "Create a two-way binding on a form input element. Data is synced on every input event by default." },
            { "v-on", "Attaches an event listener to the element. The event type is denoted by the argument. It is also the only directive that can be used with the key filter." },
            { "v-pre", "Skip compilation for this element and all its children. Skipping large numbers of nodes with no directives on them can speed up compilation." },
            { "v-ref", "Register a reference to a child component on its parent for easier access. Only respected when used on a component or with v-repeat. The component instance will be accessible on its parent’s $ object" },
            { "v-repeat", "Create a child ViewModel for every item in the binding Array or Object. If the value is a whole Number then that many child ViewModels are created. These child ViewModels will be automatically created / destroyed when mutating methods, e.g. push(), are called on the Array or Object, or the number is increased or decreased." },
            { "v-show", "Set the element’s display to none or its original value, depending on the truthy-ness of the binding's value." },
            { "v-style", "Apply inline CSS styles to the element. When there is no argument, the bound value can either be a String or an Object." },
            { "v-text", "Updates the element's textContent. Internally, {{ Mustache }} interpolations are also compiled as a v-text directive on a textNode." },
            { "v-transition", "Notify Vue.js to apply transitions to this element. The transition classes are applied when certain transition-triggering directives modify the element, or when the Vue instance’s DOM manipulation methods are called." },
        };

        public override string CompletionType
        {
            get { return CompletionTypes.Attributes; }
        }

        public override IList<HtmlCompletion> GetEntries(HtmlCompletionContext context)
        {
            var list = new List<HtmlCompletion>();

            foreach (var name in _attributes.Keys)
            {
                list.Add(CreateItem(name, _attributes[name], context.Session));
            }

            return list;
        }
    }
}