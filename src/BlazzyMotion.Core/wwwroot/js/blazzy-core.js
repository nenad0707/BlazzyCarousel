let swiperLoaded = false;
const swiperInstances = new Map();

// script/stylesheet loaders


function loadScript(src) {
    return new Promise((resolve, reject) => {
        const existing = document.querySelector(`script[src="${src}"]`);

        if (existing) {
            if (typeof Swiper !== "undefined") {
                resolve();
                return;
            }

            const check = setInterval(() => {
                if (typeof Swiper !== "undefined") {
                    clearInterval(check);
                    resolve();
                }
            }, 100);
            return;
        }

        const script = document.createElement("script");
        script.src = src;
        script.async = false;

        script.onload = () => {
            let tries = 0;

            const check = setInterval(() => {
                tries++;

                if (typeof Swiper !== "undefined") {
                    clearInterval(check);
                    resolve();
                }
                else if (tries > 50) {
                    clearInterval(check);
                    reject(new Error("Swiper script loaded but not defined"));
                }
            }, 100);
        };

        script.onerror = () => reject(new Error(`Failed to load script: ${src}`));
        document.head.appendChild(script);
    });
}

function loadStylesheet(href) {
    return new Promise((resolve, reject) => {
        if (document.querySelector(`link[href="${href}"]`)) {
            resolve();
            return;
        }

        const link = document.createElement("link");
        link.rel = "stylesheet";
        link.href = href;
        link.onload = () => resolve();
        link.onerror = () => reject(new Error(`Failed to load stylesheet: ${href}`));
        document.head.appendChild(link);
    });
}


// swper loading

export async function ensureSwiperLoaded() {
    if (swiperLoaded) return;

    await Promise.all([
        loadStylesheet("_content/BlazzyMotion.Core/css/swiper-bundle.min.css"),
        loadStylesheet("_content/BlazzyMotion.Core/css/blazzy-core.css"),
    ]);

    await loadScript("_content/BlazzyMotion.Core/js/swiper-bundle.min.js");

    if (typeof Swiper === "undefined") {
        throw new Error("Swiper is not defined after load");
    }

    swiperLoaded = true;
}


const DEFAULT_TOUCH_SETTINGS = {
    touchRatio: 1.0,
    threshold: 10,
    shortSwipes: false,
    resistanceRatio: 0.85,
    longSwipesRatio: 0.3
};


// CAROUSEL INITIALIZATION


export async function initializeCarousel(element, optionsJson, dotNetRef = null) {
    try {
        const container = element.querySelector(".swiper-container");

        if (!container) {
            console.warn("[BlazzyMotion] No .swiper-container found in element");
            return;
        }

        // Destroy existing instance if present
        if (swiperInstances.has(element)) {
            const instance = swiperInstances.get(element);
            instance.destroy(true, true);
            swiperInstances.delete(element);
        }

        const options = optionsJson ? JSON.parse(optionsJson) : {};
        const wrapper = container.querySelector('.swiper-wrapper');
        const originalSlides = wrapper.querySelectorAll('.swiper-slide');
        const slideCount = originalSlides.length;


        const minSlidesForLoop = 4;
        const minSlidesForAutoLoop = 7;
        const shouldLoop = options.loop === true && slideCount >= minSlidesForLoop;

        if (shouldLoop && slideCount < minSlidesForAutoLoop) {
            const slidesToAdd = minSlidesForAutoLoop - slideCount + 2;
            for (let i = 0; i < slidesToAdd; i++) {
                const clone = originalSlides[i % slideCount].cloneNode(true);
                clone.setAttribute('data-bz-clone', 'true');
                wrapper.appendChild(clone);
            }
        }


        const touchRatio = options.touchRatio ?? DEFAULT_TOUCH_SETTINGS.touchRatio;
        const threshold = options.threshold ?? DEFAULT_TOUCH_SETTINGS.threshold;
        const shortSwipes = options.shortSwipes ?? DEFAULT_TOUCH_SETTINGS.shortSwipes;
        const resistanceRatio = options.resistanceRatio ?? DEFAULT_TOUCH_SETTINGS.resistanceRatio;
        const longSwipesRatio = options.longSwipesRatio ?? DEFAULT_TOUCH_SETTINGS.longSwipesRatio;

        const swiperConfig = {
            // Effect settings
            effect: options.effect || "coverflow",
            grabCursor: options.grabCursor ?? true,
            centeredSlides: options.centeredSlides ?? true,
            slidesPerView: options.slidesPerView || "auto",
            initialSlide: options.initialSlide || 0,
            loop: shouldLoop,
            speed: 0,
            runCallbacksOnInit: false,
            slideToClickedSlide: true,
            watchSlidesProgress: true,
            observer: true,
            observeParents: true,
            // Touch settings - mobile optimization
            touchRatio: touchRatio,
            threshold: threshold,
            shortSwipes: shortSwipes,
            resistanceRatio: resistanceRatio,
            longSwipesRatio: longSwipesRatio,


            coverflowEffect: {
                rotate: options.rotateDegree || 50,
                stretch: options.stretch || 0,
                depth: options.depth || 150,
                modifier: options.modifier || 1.5,
                slideShadows: options.slideShadows ?? true,
            },

            on: {
                setTranslate: function () {
                    // Fix z-index issues
                    this.slides.forEach(slide => {
                        const currentZ = parseInt(slide.style.zIndex);

                        if (currentZ < 0 || isNaN(currentZ)) {
                            slide.style.zIndex = '1';
                        }

                        slide.style.pointerEvents = 'auto';
                    });

                    // Enable speed after initial render
                    if (this.params.speed === 0) {
                        this.params.speed = options.speed || 300;
                        this.params.runCallbacksOnInit = true;

                        // Show carousel after initialization
                        setTimeout(() => {
                            if (this.el) {
                                this.el.classList.remove('bzc-hidden');
                                this.el.classList.add('bzc-visible');
                            }
                        }, 100);
                    }
                },


                slideChange: function () {
                    if (dotNetRef) {
                        const realIndex = this.realIndex;
                        dotNetRef.invokeMethodAsync('OnSlideChangeFromJS', realIndex)
                            .catch(err => {
                                // Ignore errors during disposal
                                if (!err.message?.includes('disposed')) {
                                    console.warn('[BlazzyMotion] slideChange callback error:', err);
                                }
                            });
                    }
                },

                // Prevent multiple rapid transitions
                touchStart: function () {
                    // Add flag to track touch state
                    this.touchStartTime = Date.now();
                },

                touchEnd: function () {
                    // Calculate touch duration for analytics/debugging
                    const touchDuration = Date.now() - (this.touchStartTime || 0);
                }
            }
        };

        if (shouldLoop) {
            swiperConfig.loopedSlides = slideCount;
            swiperConfig.loopAdditionalSlides = 2;
        }

        const swiperInstance = new Swiper(container, swiperConfig);

        // Store instance for later reference
        swiperInstances.set(element, swiperInstance);

    } catch (err) {
        console.error("[BlazzyMotion] Initialization error:", err);
    }
}


export function destroyCarousel(element) {
    if (swiperInstances.has(element)) {
        const instance = swiperInstances.get(element);
        instance.destroy(true, true);
        swiperInstances.delete(element);
    }
}

export function getActiveIndex(element) {
    if (swiperInstances.has(element)) {
        return swiperInstances.get(element).activeIndex;
    }
    return 0;
}

export function slideTo(element, index, speed = 300) {
    if (swiperInstances.has(element)) {
        swiperInstances.get(element).slideTo(index, speed);
    }
}

export function slideNext(element, speed = 300) {
    if (swiperInstances.has(element)) {
        swiperInstances.get(element).slideNext(speed);
    }
}

export function slidePrev(element, speed = 300) {
    if (swiperInstances.has(element)) {
        swiperInstances.get(element).slidePrev(speed);
    }
}