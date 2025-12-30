<template>
  <div class="mesh-card bg-md-surface rounded-3xl p-8 mb-6" style="box-shadow:0 1px 3px rgba(0,0,0,0.12),0 1px 2px rgba(0,0,0,0.24)">
      <h2 class="text-xl font-bold text-md-on-surface mb-6 flex items-center gap-2">
            <i class="fas fa-info-circle text-md-primary"></i>
            Basic Information
          </h2>
      <div class="space-y-6">

        <div>
          <label class="block text-sm font-semibold text-md-on-surface mb-2">Bundle Name *</label>
          <input type="text" placeholder="e.g., Family Meal Deal" value="Family Meal Deal"
            class="w-full px-4 py-3 border rounded-2xl focus:outline-none focus:ring-2 focus:ring-[#1E8E3E] focus:border-transparent">
        </div>

        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-semibold text-md-on-surface mb-2">Base Price *</label>
            <div class="relative">
              <span class="absolute left-4 top-1/2 -translate-y-1/2 text-gray-500 font-semibold">R</span>
              <input type="number" value="250"
                class="w-full pl-10 pr-4 py-3 border rounded-2xl focus:outline-none focus:ring-2 focus:ring-[#1E8E3E] focus:border-transparent font-bold text-lg">
            </div>
          </div>
          <div>
            <label class="block text-sm font-semibold text-md-on-surface mb-2">Bundle Type</label>
            <select
              class="w-full px-4 py-3 border rounded-2xl focus:outline-none focus:ring-2 focus:ring-[#1E8E3E] focus:border-transparent bg-md-surface">
              <option selected="">Meal Deal</option>
              <option>Family Package</option>
              <option>Party Bundle</option>
              <option>Custom</option>
            </select>
          </div>
        </div>

        <div>
          <label class="block text-sm font-semibold text-md-on-surface mb-2">Description</label>
          <textarea rows="2" placeholder="Describe this bundle..."
            class="w-full px-4 py-3 border rounded-2xl focus:outline-none focus:ring-2 focus:ring-[#1E8E3E] focus:border-transparent resize-none">Perfect for families - includes 2 large pizzas, sides and drinks</textarea>
        </div>

        <div>
          <label class="block text-sm font-semibold text-md-on-surface mb-3">Available In *</label>
          <div class="flex gap-3 flex-wrap">
            <label
              class="flex items-center gap-2 px-4 py-2 border-[#1E8E3E] text-md-on-surface border border-md-outline-variant rounded-2xl cursor-pointer">
              <input type="checkbox" checked="" class="w-4 h-4 rounded border text-[#1E8E3E]" value="on">
              <span class="font-semibold text-md-on-surface text-sm">Grand Dining</span>
            </label>
            <label
              class="flex items-center gap-2 px-4 py-2 text-md-on-surface border border-md-outline-variant rounded-2xl cursor-pointer hover:border-[#1E8E3E]">
              <input type="checkbox" class="w-4 h-4 rounded border text-[#1E8E3E]" value="on">
              <span class="font-semibold text-md-on-surface text-sm">Velvet Lounge</span>
            </label>
            <label
              class="flex items-center gap-2 px-4 py-2 border-gray-200 rounded-2xl cursor-pointer hover:border-[#1E8E3E] text-md-on-surface border border-md-outline-variant">
              <input type="checkbox" class="w-4 h-4 rounded border text-[#1E8E3E]" value="on">
              <span class="font-semibold text-md-on-surface text-sm">Crystal Gardens</span>
            </label>
          </div>
        </div>
      </div>
    </div>

    <div class="bg-md-surface rounded-3xl p-8 mb-6" style="box-shadow:0 1px 3px rgba(0,0,0,0.12),0 1px 2px rgba(0,0,0,0.24)">
      <div class="flex items-center justify-between mb-6">
        <div>
          <h2 class="text-2xl font-bold text-md-on-surface">Bundle Steps</h2>
          <p class="text-sm text-gray-600 mt-1">
            Define the choices customers make
          </p>
        </div>
        <button
          class="inline-flex items-center gap-2 px-5 py-3 bg-[#1E8E3E] text-white rounded-full font-semibold hover:shadow-lg transition-all duration-200">
          <i class="fas fa-plus" aria-hidden="true"></i>
          <span>Add Step</span>
        </button>
      </div>

      <div class="border-[#1E8E3E] rounded-3xl p-6 mb-4" role="step">
        <div class="flex items-start justify-between mb-4">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 bg-[#1E8E3E] text-white rounded-full flex items-center justify-center font-bold">
              1
            </div>
            <div class="flex-1">
              <input type="text" value="Choose First Pizza"
                class="text-xl font-bold text-md-on-surface border-0 border-b-2 border-transparent hover:border-gray-300 focus:border-[#1E8E3E] focus:outline-none px-2 py-1 -ml-2">
              <p class="text-sm text-gray-600 mt-1">
                Customer selects one product from allowed list
              </p>
            </div>
          </div>
          <div class="flex gap-2">
            <button class="w-8 h-8 rounded-lg hover:bg-[#F5F5F5] text-gray-600 flex items-center justify-center">
              <i class="fas fa-arrows-alt text-sm" aria-hidden="true"></i>
            </button>
            <button
              class="w-8 h-8 rounded-lg hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center">
              <i class="fas fa-trash text-sm" aria-hidden="true"></i>
            </button>
          </div>
        </div>

        <div class="bg-md-surface-container border border-md-outline-variant rounded-2xl p-4 mb-4" role="allowed-products">
          <div class="flex items-center justify-between mb-3">
            <h4 class="font-semibold text-md-on-surface flex items-center gap-2">
              <i class="fas fa-check-circle text-[#1E8E3E]" aria-hidden="true"></i>
              Allowed Products (3)
            </h4>
            <button class="text-sm text-[#1E8E3E] hover:text-[#137a3c] font-semibold">
              <i class="fas fa-plus mr-1" aria-hidden="true"></i>Add Products
            </button>
          </div>
          <div class="space-y-2">
            <div class="flex items-center justify-between bg-md-surface p-3 rounded-xl border border-md-outline-variant gap-3">
              <i class="fas fa-grip-vertical text-gray-400" aria-hidden="true"></i>
              <div class="w-10 h-10 bg-[#FCE8E6] rounded-lg flex items-center justify-center">
                <i class="fas fa-pizza-slice text-[#C5221F]" aria-hidden="true"></i>
              </div>
              <div class="flex-1">
                <p class="font-semibold text-md-on-surface text-sm">
                  Bacon, Avo &amp; Feta Pizza
                </p>
                <p class="text-xs text-gray-600">Large size included</p>
              </div>
              <button
                class="w-6 h-6 rounded-full hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center">
                <i class="fas fa-times text-xs" aria-hidden="true"></i>
              </button>
            </div>
            <div class="flex items-center justify-between bg-md-surface p-3 rounded-xl border border-md-outline-variant gap-3">
              <i class="fas fa-grip-vertical text-gray-400" aria-hidden="true"></i>
              <div class="w-10 h-10 bg-[#FEF7E0] rounded-lg flex items-center justify-center">
                <i class="fas fa-pizza-slice text-[#F9AB00]" aria-hidden="true"></i>
              </div>
              <div class="flex-1">
                <p class="font-semibold text-md-on-surface text-sm">
                  Classic Cheese Pizza
                </p>
                <p class="text-xs text-gray-600">Large size included</p>
              </div>
              <button
                class="w-6 h-6 rounded-full hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center">
                <i class="fas fa-times text-xs" aria-hidden="true"></i>
              </button>
            </div>
            <div class="flex items-center justify-between bg-md-surface p-3 rounded-xl border border-md-outline-variant gap-3">
              <i class="fas fa-grip-vertical text-gray-400" aria-hidden="true"></i>
              <div class="w-10 h-10 bg-[#E8F0FE] rounded-lg flex items-center justify-center">
                <i class="fas fa-pizza-slice text-[#1967D2]" aria-hidden="true"></i>
              </div>
              <div class="flex-1">
                <p class="font-semibold text-md-on-surface text-sm">
                  Pepperoni Deluxe Pizza
                </p>
                <p class="text-xs text-gray-600">Large size included</p>
              </div>
              <button
                class="w-6 h-6 rounded-full hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center">
                <i class="fas fa-times text-xs" aria-hidden="true"></i>
              </button>
            </div>
          </div>
        </div>

        <div class="bg-[#E8F0FE] rounded-2xl p-4" role="option-group">
          <div class="flex items-center justify-between mb-3">
            <h4 class="font-semibold text-md-on-surface flex items-center gap-2">
              <i class="fas fa-sliders-h text-[#4A90E2]" aria-hidden="true"></i>
              Option Groups (2)
            </h4>
            <button class="text-sm text-[#4A90E2] hover:text-[#357ABD] font-semibold">
              <i class="fas fa-plus mr-1" aria-hidden="true"></i>Add Option Group
            </button>
          </div>

          <div class="bg-md-surface rounded-xl p-4 mb-3" role="option">
            <div class="flex items-center justify-between mb-3">
              <div class="flex-1">
                <div class="flex items-center gap-2 mb-1">
                  <input type="text" value="Choose Base"
                    class="font-semibold text-md-on-surface border-0 border-b-2 border-transparent hover:border-gray-300 focus:border-[#4A90E2] focus:outline-none px-1 -ml-1">
                  <span class="px-2 py-0.5 bg-[#FCE8E6] text-[#C5221F] text-xs font-bold rounded-full">REQUIRED</span>
                </div>
                <p class="text-xs text-gray-600 pl-1">Min: 1 • Max: 1</p>
              </div>
              <button
                class="w-6 h-6 rounded-full hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center">
                <i class="fas fa-trash text-xs" aria-hidden="true"></i>
              </button>
            </div>
            <div class="space-y-1">
              <div class="flex items-center justify-between p-2 bg-[#F5F5F5] rounded-lg">
                <span class="text-sm text-md-on-surface">Gluten Free</span>
                <span class="text-sm font-semibold text-[#1E8E3E]">Free</span>
              </div>
              <div class="flex items-center justify-between p-2 bg-[#F5F5F5] rounded-lg">
                <span class="text-sm text-md-on-surface">Traditional</span>
                <span class="text-sm font-semibold text-[#1E8E3E]">Free</span>
              </div>
              <div class="flex items-center justify-between p-2 bg-[#F5F5F5] rounded-lg">
                <span class="text-sm text-md-on-surface">Pan</span>
                <span class="text-sm font-semibold text-[#1E8E3E]">Free</span>
              </div>
              <button class="w-full py-2 text-sm text-[#4A90E2] hover:text-[#357ABD] font-semibold">
                + Add Option
              </button>
            </div>
          </div>

          <div class="bg-md-surface rounded-xl p-4">
            <div class="flex items-center justify-between mb-3">
              <div class="flex-1">
                <div class="flex items-center gap-2 mb-1">
                  <input type="text" value="Extra Meat Toppings"
                    class="font-semibold text-md-on-surface border-0 border-b-2 border-transparent hover:border-gray-300 focus:border-[#4A90E2] focus:outline-none px-1 -ml-1">
                  <span class="px-2 py-0.5 bg-[#E6F4EA] text-[#1E8E3E] text-xs font-bold rounded-full">OPTIONAL</span>
                </div>
                <p class="text-xs text-gray-600 pl-1">Min: 0 • Max: 10</p>
              </div>
              <button
                class="w-6 h-6 rounded-full hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center">
                <i class="fas fa-trash text-xs" aria-hidden="true"></i>
              </button>
            </div>
            <div class="space-y-1">
              <div class="flex items-center justify-between p-2 bg-[#F5F5F5] rounded-lg">
                <span class="text-sm text-md-on-surface">Bacon</span>
                <span class="text-sm font-semibold text-md-on-surface">+ R 28.90</span>
              </div>
              <div class="flex items-center justify-between p-2 bg-[#F5F5F5] rounded-lg">
                <span class="text-sm text-md-on-surface">Chicken</span>
                <span class="text-sm font-semibold text-md-on-surface">+ R 28.90</span>
              </div>
              <div class="flex items-center justify-between p-2 bg-[#F5F5F5] rounded-lg">
                <span class="text-sm text-md-on-surface">Ham</span>
                <span class="text-sm font-semibold text-md-on-surface">+ R 28.90</span>
              </div>
              <button class="w-full py-2 text-sm text-[#4A90E2] hover:text-[#357ABD] font-semibold">
                + Add Option
              </button>
            </div>
          </div>
        </div>
      </div>

      <div class="border-gray-200 rounded-3xl p-6 mb-4 hover:border-[#1E8E3E] transition-all" role="step">
        <div class="flex items-start justify-between mb-4">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 bg-gray-300 text-white rounded-full flex items-center justify-center font-bold">
              2
            </div>
            <div class="flex-1">
              <input type="text" value="Choose Second Pizza"
                class="text-xl font-bold text-md-on-surface border-0 border-b-2 border-transparent hover:border-gray-300 focus:border-[#1E8E3E] focus:outline-none px-2 py-1 -ml-2">
              <p class="text-sm text-gray-600 mt-1">Same structure as Step 1</p>
            </div>
          </div>
          <div class="flex gap-2">
            <button class="w-8 h-8 rounded-lg hover:bg-[#F5F5F5] text-gray-600 flex items-center justify-center">
              <i class="fas fa-arrows-alt text-sm" aria-hidden="true"></i>
            </button>
            <button
              class="w-8 h-8 rounded-lg hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center">
              <i class="fas fa-trash text-sm" aria-hidden="true"></i>
            </button>
          </div>
        </div>
              <div class="bg-md-surface-container border border-md-outline-variant rounded-2xl p-4 mb-4" role="allowed-products">
          <div class="flex items-center justify-between mb-3">
            <h4 class="font-semibold text-md-on-surface flex items-center gap-2">
              <i class="fas fa-check-circle text-[#1E8E3E]" aria-hidden="true"></i>
              Allowed Products (3)
            </h4>
            <button class="text-sm text-[#1E8E3E] hover:text-[#137a3c] font-semibold">
              <i class="fas fa-plus mr-1" aria-hidden="true"></i>Add Products
            </button>
          </div>
          <div class="space-y-2">
            <div class="flex items-center justify-between bg-md-surface p-3 rounded-xl border border-md-outline-variant gap-3">
              <i class="fas fa-grip-vertical text-gray-400" aria-hidden="true"></i>
              <div class="w-10 h-10 bg-[#FCE8E6] rounded-lg flex items-center justify-center">
                <i class="fas fa-pizza-slice text-[#C5221F]" aria-hidden="true"></i>
              </div>
              <div class="flex-1">
                <p class="font-semibold text-md-on-surface text-sm">
                  Bacon, Avo &amp; Feta Pizza
                </p>
                <p class="text-xs text-gray-600">Large size included</p>
              </div>
              <button
                class="w-6 h-6 rounded-full hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center">
                <i class="fas fa-times text-xs" aria-hidden="true"></i>
              </button>
            </div>
            <div class="flex items-center justify-between bg-md-surface p-3 rounded-xl border border-md-outline-variant gap-3">
              <i class="fas fa-grip-vertical text-gray-400" aria-hidden="true"></i>
              <div class="w-10 h-10 bg-[#FEF7E0] rounded-lg flex items-center justify-center">
                <i class="fas fa-pizza-slice text-[#F9AB00]" aria-hidden="true"></i>
              </div>
              <div class="flex-1">
                <p class="font-semibold text-md-on-surface text-sm">
                  Classic Cheese Pizza
                </p>
                <p class="text-xs text-gray-600">Large size included</p>
              </div>
              <button
                class="w-6 h-6 rounded-full hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center">
                <i class="fas fa-times text-xs" aria-hidden="true"></i>
              </button>
            </div>
            <div class="flex items-center justify-between bg-md-surface p-3 rounded-xl border border-md-outline-variant gap-3">
              <i class="fas fa-grip-vertical text-gray-400" aria-hidden="true"></i>
              <div class="w-10 h-10 bg-[#E8F0FE] rounded-lg flex items-center justify-center">
                <i class="fas fa-pizza-slice text-[#1967D2]" aria-hidden="true"></i>
              </div>
              <div class="flex-1">
                <p class="font-semibold text-md-on-surface text-sm">
                  Pepperoni Deluxe Pizza
                </p>
                <p class="text-xs text-gray-600">Large size included</p>
              </div>
              <button
                class="w-6 h-6 rounded-full hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center">
                <i class="fas fa-times text-xs" aria-hidden="true"></i>
              </button>
            </div>
          </div>
        </div>

        <div class="bg-[#E8F0FE] rounded-2xl p-4" role="option-group">
          <div class="flex items-center justify-between mb-3">
            <h4 class="font-semibold text-md-on-surface flex items-center gap-2">
              <i class="fas fa-sliders-h text-[#4A90E2]" aria-hidden="true"></i>
              Option Groups (2)
            </h4>
            <button class="text-sm text-[#4A90E2] hover:text-[#357ABD] font-semibold">
              <i class="fas fa-plus mr-1" aria-hidden="true"></i>Add Option Group
            </button>
          </div>

          <div class="bg-md-surface rounded-xl p-4 mb-3" role="option">
            <div class="flex items-center justify-between mb-3">
              <div class="flex-1">
                <div class="flex items-center gap-2 mb-1">
                  <input type="text" value="Choose Base"
                    class="font-semibold text-md-on-surface border-0 border-b-2 border-transparent hover:border-gray-300 focus:border-[#4A90E2] focus:outline-none px-1 -ml-1">
                  <span class="px-2 py-0.5 bg-[#FCE8E6] text-[#C5221F] text-xs font-bold rounded-full">REQUIRED</span>
                </div>
                <p class="text-xs text-gray-600 pl-1">Min: 1 • Max: 1</p>
              </div>
              <button
                class="w-6 h-6 rounded-full hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center">
                <i class="fas fa-trash text-xs" aria-hidden="true"></i>
              </button>
            </div>
            <div class="space-y-1">
              <div class="flex items-center justify-between p-2 bg-[#F5F5F5] rounded-lg">
                <span class="text-sm text-md-on-surface">Gluten Free</span>
                <span class="text-sm font-semibold text-[#1E8E3E]">Free</span>
              </div>
              <div class="flex items-center justify-between p-2 bg-[#F5F5F5] rounded-lg">
                <span class="text-sm text-md-on-surface">Traditional</span>
                <span class="text-sm font-semibold text-[#1E8E3E]">Free</span>
              </div>
              <div class="flex items-center justify-between p-2 bg-[#F5F5F5] rounded-lg">
                <span class="text-sm text-md-on-surface">Pan</span>
                <span class="text-sm font-semibold text-[#1E8E3E]">Free</span>
              </div>
              <button class="w-full py-2 text-sm text-[#4A90E2] hover:text-[#357ABD] font-semibold">
                + Add Option
              </button>
            </div>
          </div>

          <div class="bg-md-surface rounded-xl p-4">
            <div class="flex items-center justify-between mb-3">
              <div class="flex-1">
                <div class="flex items-center gap-2 mb-1">
                  <input type="text" value="Extra Meat Toppings"
                    class="font-semibold text-md-on-surface border-0 border-b-2 border-transparent hover:border-gray-300 focus:border-[#4A90E2] focus:outline-none px-1 -ml-1">
                  <span class="px-2 py-0.5 bg-[#E6F4EA] text-[#1E8E3E] text-xs font-bold rounded-full">OPTIONAL</span>
                </div>
                <p class="text-xs text-gray-600 pl-1">Min: 0 • Max: 10</p>
              </div>
              <button
                class="w-6 h-6 rounded-full hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center">
                <i class="fas fa-trash text-xs" aria-hidden="true"></i>
              </button>
            </div>
            <div class="space-y-1">
              <div class="flex items-center justify-between p-2 bg-[#F5F5F5] rounded-lg">
                <span class="text-sm text-md-on-surface">Bacon</span>
                <span class="text-sm font-semibold text-md-on-surface">+ R 28.90</span>
              </div>
              <div class="flex items-center justify-between p-2 bg-[#F5F5F5] rounded-lg">
                <span class="text-sm text-md-on-surface">Chicken</span>
                <span class="text-sm font-semibold text-md-on-surface">+ R 28.90</span>
              </div>
              <div class="flex items-center justify-between p-2 bg-[#F5F5F5] rounded-lg">
                <span class="text-sm text-md-on-surface">Ham</span>
                <span class="text-sm font-semibold text-md-on-surface">+ R 28.90</span>
              </div>
              <button class="w-full py-2 text-sm text-[#4A90E2] hover:text-[#357ABD] font-semibold">
                + Add Option
              </button>
            </div>
          </div>
        </div>
      </div>

      <div class="border-gray-200 rounded-3xl p-6 hover:border-[#1E8E3E] transition-all" role="step">
        <div class="flex items-start justify-between mb-4">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 bg-gray-300 text-white rounded-full flex items-center justify-center font-bold">
              3
            </div>
            <div class="flex-1">
              <input type="text" value="Choose Cold Drink"
                class="text-xl font-bold text-md-on-surface border-0 border-b-2 border-transparent hover:border-gray-300 focus:border-[#1E8E3E] focus:outline-none px-2 py-1 -ml-2">
              <p class="text-sm text-gray-600 mt-1">
                Simple product selection - no options needed
              </p>
            </div>
          </div>
          <div class="flex gap-2">
            <button class="w-8 h-8 rounded-lg hover:bg-[#F5F5F5] text-gray-600 flex items-center justify-center">
              <i class="fas fa-arrows-alt text-sm" aria-hidden="true"></i>
            </button>
            <button
              class="w-8 h-8 rounded-lg hover:bg-[#FCE8E6] text-gray-600 hover:text-[#C5221F] flex items-center justify-center">
              <i class="fas fa-trash text-sm" aria-hidden="true"></i>
            </button>
          </div>
        </div>
        <p class="text-sm text-gray-600 bg-[#F5F5F5] rounded-xl p-4">
          <i class="fas fa-info-circle text-[#4A90E2] mr-2" aria-hidden="true"></i>
          Click to expand and add drink options
        </p>
      </div>
    </div>

    <div class="bg-gradient-to-br from-[#E6F4EA] to-[#E8F0FE] rounded-3xl p-6 mb-6"
      style="box-shadow:0 1px 3px rgba(0,0,0,0.12),0 1px 2px rgba(0,0,0,0.24)">
      <div class="flex items-start gap-4">
        <div class="w-12 h-12 bg-md-surface rounded-2xl flex items-center justify-center flex-shrink-0">
          <i class="fas fa-check-circle text-[#1E8E3E] text-xl" aria-hidden="true"></i>
        </div>
        <div class="flex-1">
          <h3 class="font-bold text-md-on-surface text-xl mb-2">Bundle Summary</h3>
          <div class="grid grid-cols-2 sm:grid-cols-4 gap-4 mb-4">
            <div>
              <p class="text-xs text-gray-600 mb-1">Base Price</p>
              <p class="font-bold text-md-on-surface text-lg">R 250.00</p>
            </div>
            <div>
              <p class="text-xs text-gray-600 mb-1">Steps</p>
              <p class="font-bold text-md-on-surface text-lg">3</p>
            </div>
            <div>
              <p class="text-xs text-gray-600 mb-1">Products</p>
              <p class="font-bold text-md-on-surface text-lg">8</p>
            </div>
            <div>
              <p class="text-xs text-gray-600 mb-1">Option Groups</p>
              <p class="font-bold text-md-on-surface text-lg">4</p>
            </div>
          </div>
          <p class="text-sm text-md-on-surface">
            Customers will make 3 choices, with customization options available
            for each selection
          </p>
        </div>
      </div>
    </div>
  <div class="p-4 md:p-8">
    <div class="max-w-7xl mx-auto">
      <!-- Loading State -->
      <div v-if="loading" class="flex items-center justify-center py-20">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-md-primary"></div>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="text-center py-20">
        <i class="fas fa-exclamation-triangle text-6xl text-md-error mb-4"></i>
        <h2 class="text-2xl font-bold text-md-on-surface mb-2">{{ error }}</h2>
        <NuxtLink to="/menu/bundles" class="text-md-primary hover:underline">
          <i class="fas fa-arrow-left mr-2"></i>
          Back to Bundles
        </NuxtLink>
      </div>

      <!-- Edit Form -->
      <div v-else>
        <!-- Header with Actions -->
        <div class="flex items-center justify-between mb-8">
          <div>
            <h1 class="text-3xl font-bold text-md-on-surface mb-2">Edit Bundle</h1>
            <p class="text-md-on-surface-variant">Manage your bundle details, steps, and products</p>
          </div>
          <div class="flex gap-3">
            <NuxtLink
              to="/menu/bundles"
              class="px-6 py-3 bg-md-surface-container border border-md-outline-variant text-md-on-surface rounded-xl hover:bg-md-surface-container-high transition-all"
            >
              Cancel
            </NuxtLink>
            <button
              @click="saveBundle"
              :disabled="saving"
              class="px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2 hover:shadow-md-4 transition-all disabled:opacity-50"
            >
              <i v-if="!saving" class="fas fa-save mr-2"></i>
              <i v-else class="fas fa-spinner fa-spin mr-2"></i>
              {{ saving ? 'Saving...' : 'Save Changes' }}
            </button>
          </div>
        </div>

        <!-- Basic Information Card -->
        <div class="mesh-card bg-md-surface rounded-2xl shadow-md-3 p-6 mb-6">
          <h2 class="text-xl font-bold text-md-on-surface mb-6 flex items-center gap-2">
            <i class="fas fa-info-circle text-md-primary"></i>
            Basic Information
          </h2>

          <div class="grid md:grid-cols-2 gap-6">
            <!-- Bundle Name -->
            <div>
              <label class="block text-sm font-bold text-md-on-surface mb-2">
                Bundle Name <span class="text-md-error">*</span>
              </label>
              <input
                v-model="bundleData.name"
                type="text"
                class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary"
                placeholder="e.g., Family Meal Deal"
              />
            </div>

            <!-- Base Price -->
            <div>
              <label class="block text-sm font-bold text-md-on-surface mb-2">
                Base Price <span class="text-md-error">*</span>
              </label>
              <div class="relative">
                <span class="absolute left-4 top-3.5 text-md-on-surface-variant">R</span>
                <input
                  v-model.number="bundleData.basePrice"
                  type="number"
                  step="0.01"
                  class="w-full pl-10 pr-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary"
                  placeholder="250.00"
                />
              </div>
            </div>

            <!-- Description -->
            <div class="md:col-span-2">
              <label class="block text-sm font-bold text-md-on-surface mb-2">
                Description
              </label>
              <textarea
                v-model="bundleData.description"
                rows="3"
                class="w-full px-4 py-3 bg-md-surface-container border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary"
                placeholder="Describe your bundle..."
              ></textarea>
            </div>

            <!-- Image Upload -->
            <div class="md:col-span-2">
              <label class="block text-sm font-bold text-md-on-surface mb-2">
                Bundle Image
              </label>
              <div class="flex items-start gap-4">
                <!-- Current Image -->
                <div v-if="bundleData.images" class="w-32 h-32 rounded-lg overflow-hidden border-2 border-md-outline-variant">
                  <img :src="bundleData.images" alt="Bundle image" class="w-full h-full object-cover" />
                </div>
                <div v-else class="w-32 h-32 rounded-lg bg-md-surface-container flex items-center justify-center border-2 border-dashed border-md-outline-variant">
                  <i class="fas fa-image text-4xl text-md-on-surface-variant opacity-50"></i>
                </div>

                <!-- Upload Button -->
                <div class="flex-1">
                  <input
                    ref="fileInput"
                    type="file"
                    accept="image/*"
                    @change="handleImageUpload"
                    class="hidden"
                  />
                  <button
                    @click="$refs.fileInput.click()"
                    class="px-4 py-2 bg-md-primary-container text-md-on-primary-container rounded-xl hover:shadow-md-2 transition-all"
                  >
                    <i class="fas fa-upload mr-2"></i>
                    Upload Image
                  </button>
                  <p class="text-xs text-md-on-surface-variant mt-2">
                    Recommended: 800x600px, JPG or PNG
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Steps Management Card -->
        <div class="mesh-card bg-md-surface rounded-2xl shadow-md-3 p-6">
          <div class="flex items-center justify-between mb-6">
            <h2 class="text-xl font-bold text-md-on-surface flex items-center gap-2">
              <i class="fas fa-layer-group text-md-primary"></i>
              Bundle Steps
            </h2>
            <button
              @click="addStep"
              class="px-4 py-2 bg-md-success-container text-md-on-success-container rounded-xl hover:bg-md-success hover:text-md-on-success transition-colors"
            >
              <i class="fas fa-plus mr-2"></i>
              Add Step
            </button>
          </div>

          <!-- Empty State -->
          <div v-if="bundleData.steps.length === 0" class="text-center py-12 bg-md-surface-container rounded-xl border-2 border-dashed border-md-outline-variant">
            <i class="fas fa-layer-group text-5xl text-md-on-surface-variant opacity-50 mb-4"></i>
            <p class="text-md-on-surface-variant mb-4">No steps added yet</p>
            <button
              @click="addStep"
              class="px-6 py-3 btn-gradient text-white rounded-xl shadow-md-2"
            >
              <i class="fas fa-plus mr-2"></i>
              Add Your First Step
            </button>
          </div>

          <!-- Steps List with Drag and Drop -->
          <div v-else class="space-y-4">
            <draggable
              v-model="bundleData.steps"
              :item-key="step => step.id"
              handle=".drag-handle"
              @end="onStepReorder"
              class="space-y-4"
            >
              <template #item="{ element: step, index }">
                <div class="bg-md-surface-container border border-md-outline-variant rounded-xl p-6">
                  <!-- Step Header -->
                  <div class="flex items-start gap-4 mb-4">
                    <!-- Drag Handle -->
                    <div class="drag-handle cursor-move p-2 hover:bg-md-surface-container-high rounded transition-colors">
                      <i class="fas fa-grip-vertical text-md-on-surface-variant"></i>
                    </div>

                    <!-- Step Number Badge -->
                    <div class="w-10 h-10 btn-gradient text-white rounded-full flex items-center justify-center font-bold text-sm shadow-md-2 flex-shrink-0">
                      {{ index + 1 }}
                    </div>

                    <!-- Step Name Input -->
                    <div class="flex-1">
                      <input
                        v-model="step.name"
                        type="text"
                        class="w-full px-4 py-2 bg-md-surface border border-md-outline-variant rounded-xl focus:ring-2 focus:ring-md-primary font-semibold"
                        placeholder="Step name (e.g., Choose Your Pizza)"
                      />
                    </div>

                    <!-- Delete Step Button -->
                    <button
                      @click="deleteStep(step, index)"
                      class="p-2 text-md-error hover:bg-md-error-container rounded-xl transition-colors"
                    >
                      <i class="fas fa-trash"></i>
                    </button>
                  </div>

                  <!-- Products in Step -->
                  <div class="ml-14">
                    <div class="flex items-center justify-between mb-3">
                      <label class="text-sm font-bold text-md-on-surface">
                        Products in this step
                      </label>
                      <div class="relative">
                        <select
                          @change="addProductToStep(step, $event)"
                          class="px-4 py-2 bg-md-surface-container-high border border-md-outline-variant rounded-xl text-sm focus:ring-2 focus:ring-md-primary"
                        >
                          <option value="">+ Add Product</option>
                          <option
                            v-for="product in availableProducts"
                            :key="product.id"
                            :value="product.id"
                            :disabled="stepHasProduct(step, product.id)"
                          >
                            {{ product.name }}
                          </option>
                        </select>
                      </div>
                    </div>

                    <!-- Products List -->
                    <div v-if="step.allowedProducts && step.allowedProducts.length > 0" class="space-y-2">
                      <div
                        v-for="(product, pIndex) in step.allowedProducts"
                        :key="product.productId || product.id"
                        class="flex items-center justify-between bg-md-surface p-3 rounded-lg border border-md-outline-variant"
                      >
                        <div class="flex items-center gap-3">
                          <i class="fas fa-grip-vertical text-md-on-surface-variant text-sm"></i>
                          <span class="text-md-on-surface font-medium">
                            {{ getProductName(product.productId || product.id) }}
                          </span>
                        </div>
                        <button
                          @click="removeProductFromStep(step, product, pIndex)"
                          class="p-2 text-md-error hover:bg-md-error-container rounded-lg transition-colors"
                        >
                          <i class="fas fa-times"></i>
                        </button>
                      </div>
                    </div>

                    <!-- Empty Products State -->
                    <div v-else class="text-center py-6 bg-md-surface rounded-lg border border-dashed border-md-outline-variant">
                      <p class="text-md-on-surface-variant text-sm">
                        No products assigned to this step yet
                      </p>
                    </div>
                  </div>
                </div>
              </template>
            </draggable>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useBundlesApi, useLibraryItemsApi, useUploadsApi } from '~/composables/useApi'
import { useToast } from '~/composables/useToast'
import draggable from 'vuedraggable'

definePageMeta({
  layout: 'menu'
})

const route = useRoute()
const router = useRouter()
const bundlesApi = useBundlesApi()
const libraryApi = useLibraryItemsApi()
const uploadsApi = useUploadsApi()
const toast = useToast()

const loading = ref(true)
const saving = ref(false)
const error = ref('')
const bundleId = ref(route.params.id as string)
const catalogId = ref('1') // Will be loaded from bundle data

const bundleData = ref({
  name: '',
  description: '',
  basePrice: 0,
  slug: '',
  images: '',
  sortOrder: 0,
  steps: [] as any[]
})

const availableProducts = ref<any[]>([])
const fileInput = ref<HTMLInputElement | null>(null)

// Stats for sidebar
const stats = ref({
  menus: 0,
  items: 0,
  categories: 0
})
provide('menuStats', stats)

// Use page metadata composable
const { setPageHeader } = usePageMeta()

onMounted(async () => {
  setPageHeader({
    title: 'Edit Bundle',
    description: 'Manage your bundle configuration'
  })

  await loadBundle()
  await loadProducts()
})

async function loadBundle() {
  try {
    loading.value = true
    const response = await bundlesApi.getBundle(bundleId.value)

    if (response.success && response.data?.bundle) {
      const bundle = response.data.bundle
      bundleData.value = {
        name: bundle.name,
        description: bundle.description || '',
        basePrice: bundle.basePrice,
        slug: bundle.slug || '',
        images: bundle.images || '',
        sortOrder: bundle.sortOrder || 0,
        steps: bundle.steps || []
      }
    } else {
      error.value = 'Bundle not found'
    }
  } catch (err: any) {
    console.error('Error loading bundle:', err)
    error.value = err.response?.data?.error || 'Failed to load bundle'
  } finally {
    loading.value = false
  }
}

async function loadProducts() {
  try {
    const response = await libraryApi.getItems()
    if (response.success && response.data?.items) {
      availableProducts.value = response.data.items
        .filter((item: any) => item.itemType === 0) // Only regular items, not bundles
        .map((item: any) => ({
          id: item.id,
          name: item.name
        }))
    }
  } catch (err) {
    console.error('Error loading products:', err)
  }
}

function getProductName(productId: number): string {
  const product = availableProducts.value.find(p => p.id === productId)
  return product?.name || 'Unknown Product'
}

function stepHasProduct(step: any, productId: number): boolean {
  if (!step.allowedProducts) return false
  return step.allowedProducts.some((p: any) => (p.productId || p.id) === productId)
}

function addStep() {
  const newStep = {
    id: Date.now(), // Temporary ID for new steps
    stepNumber: bundleData.value.steps.length + 1,
    name: '',
    minSelect: 1,
    maxSelect: 1,
    allowedProducts: [],
    isNew: true
  }
  bundleData.value.steps.push(newStep)
}

async function deleteStep(step: any, index: number) {
  if (!confirm(`Are you sure you want to delete "${step.name || 'this step'}"?`)) return

  try {
    // If step exists in DB, delete it via API
    if (!step.isNew && step.id) {
      await bundlesApi.deleteStep(bundleId.value, step.id.toString())
      toast.success('Step deleted successfully')
    }

    // Remove from local array
    bundleData.value.steps.splice(index, 1)

    // Renumber remaining steps
    bundleData.value.steps.forEach((s, i) => {
      s.stepNumber = i + 1
    })
  } catch (err) {
    console.error('Error deleting step:', err)
    toast.error('Failed to delete step')
  }
}

function addProductToStep(step: any, event: Event) {
  const target = event.target as HTMLSelectElement
  const productId = parseInt(target.value)

  if (productId && !stepHasProduct(step, productId)) {
    if (!step.allowedProducts) {
      step.allowedProducts = []
    }
    step.allowedProducts.push({
      productId,
      isNew: true
    })
  }

  target.value = '' // Reset select
}

async function removeProductFromStep(step: any, product: any, index: number) {
  try {
    const productId = product.productId || product.id

    // If product exists in DB, delete it via API
    if (!product.isNew && step.id && !step.isNew) {
      await bundlesApi.removeProductFromStep(
        bundleId.value,
        step.id.toString(),
        productId.toString()
      )
      toast.success('Product removed from step')
    }

    // Remove from local array
    step.allowedProducts.splice(index, 1)
  } catch (err) {
    console.error('Error removing product:', err)
    toast.error('Failed to remove product')
  }
}

async function onStepReorder() {
  // Update step numbers based on new order
  bundleData.value.steps.forEach((step, index) => {
    step.stepNumber = index + 1
  })

  try {
    // Send reorder request to backend
    const steps = bundleData.value.steps
      .filter(s => !s.isNew) // Only existing steps
      .map(s => ({
        stepId: s.id,
        stepNumber: s.stepNumber
      }))

    if (steps.length > 0) {
      await bundlesApi.reorderSteps(bundleId.value, { steps })
      toast.success('Steps reordered')
    }
  } catch (err) {
    console.error('Error reordering steps:', err)
    toast.error('Failed to reorder steps')
  }
}

async function handleImageUpload(event: Event) {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]

  if (!file) return

  try {
    const response = await uploadsApi.uploadFile(file)
    if (response.success && response.data?.url) {
      bundleData.value.images = response.data.url
      toast.success('Image uploaded successfully')
    }
  } catch (err) {
    console.error('Error uploading image:', err)
    toast.error('Failed to upload image')
  }
}

async function saveBundle() {
  try {
    saving.value = true

    // Update basic bundle info
    await bundlesApi.updateBundle(bundleId.value, {
      name: bundleData.value.name,
      description: bundleData.value.description,
      basePrice: bundleData.value.basePrice,
      slug: bundleData.value.slug,
      images: bundleData.value.images,
      sortOrder: bundleData.value.sortOrder
    })

    // Process new steps
    for (const step of bundleData.value.steps) {
      if (step.isNew) {
        // Create new step
        const stepResponse = await bundlesApi.addStep(bundleId.value, {
          stepNumber: step.stepNumber,
          name: step.name,
          minSelect: step.minSelect,
          maxSelect: step.maxSelect
        })

        const newStepId = stepResponse.data.step.id
        step.id = newStepId
        step.isNew = false

        // Add products to the new step
        if (step.allowedProducts) {
          for (const product of step.allowedProducts) {
            await bundlesApi.addProductToStep(
              bundleId.value,
              newStepId,
              { productId: product.productId || product.id }
            )
          }
        }
      } else {
        // Update existing step
        await bundlesApi.updateStep(bundleId.value, step.id.toString(), {
          name: step.name,
          minSelect: step.minSelect,
          maxSelect: step.maxSelect,
          stepNumber: step.stepNumber
        })

        // Add new products to existing step
        if (step.allowedProducts) {
          for (const product of step.allowedProducts) {
            if (product.isNew) {
              await bundlesApi.addProductToStep(
                bundleId.value,
                step.id,
                { productId: product.productId || product.id }
              )
              product.isNew = false
            }
          }
        }
      }
    }

    toast.success('Bundle updated successfully!')
    router.push('/menu/bundles')
  } catch (err: any) {
    console.error('Error saving bundle:', err)
    toast.error(err.response?.data?.error || 'Failed to save bundle')
  } finally {
    saving.value = false
  }
}
</script>
