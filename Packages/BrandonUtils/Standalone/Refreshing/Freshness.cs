namespace BrandonUtils.Standalone.Refreshing {
    public enum Freshness {
        /// <summary>
        /// Implies that:
        /// <ul>
        /// <li><see cref="Refreshing{TValue,TStaleness}.Refresh"/> has <b>never</b> been called.</li>
        /// <li>The value of <see cref="Refreshing{TValue,TStaleness}._value"/> is the default <see cref="TValue"/>.</li>
        /// </ul>
        /// </summary>
        /// <example>
        /// <ul>
        /// <li>An apple on the tree.</li>
        /// <li>Spam in the can.</li>
        /// </ul>
        /// </example>
        /// <remarks>
        /// Also known as <a href="https://www.youtube.com/watch?v=6PrRUKFcnPM&amp;ab_channel=yadif2x">NRFB ("Never Remove From Box")</a>.
        /// </remarks>
        Pristine = default,
        /// <summary>
        /// Implies that:
        /// <ul>
        /// <li>The result of <see cref="Refreshing{TValue,TStaleness}.StalenessPredicate"/> is <c>false</c>.</li>
        /// <li>Calling <see cref="Refreshing{TValue,TStaleness}.Value"/> will <b>not</b> trigger <see cref="Refreshing{TValue,TStaleness}.Refresh"/>.</li>
        /// </ul>
        /// </summary>
        Fresh,
        /// <summary>
        /// Implies that:
        /// <ul>
        /// <li>The result of <see cref="Refreshing{TValue,TStaleness}.StalenessPredicate"/> is <c>true</c>.</li>
        /// <li>Calling <see cref="Refreshing{TValue,TStaleness}.Value"/> will trigger <see cref="Refreshing{TValue,TStaleness}.Refresh"/>.</li>
        /// </ul>
        /// </summary>
        Stale
    }
}