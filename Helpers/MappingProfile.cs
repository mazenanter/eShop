using AutoMapper;

namespace eShop.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, RegisterDto>().ReverseMap();
            CreateMap<ApplicationUser, AuthResponseDto>();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<ProductDto, Product>()
    .ForMember(dest => dest.MainImageUrl, opt => opt.Ignore())
    .ForMember(dest => dest.ProductImages, opt => opt.Ignore());

            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.MainImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore());
            CreateMap<CartItem, CartItemDto>()
    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
    .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.MainImageUrl));

          
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems));

            CreateMap<OrderItem, OrderItemResponseDto>()
    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            
            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.OrderItems));

           
            CreateMap<Order, GetOrdersDto>();
            CreateMap<ApplicationUser, UserProfileDto>();
            CreateMap<UpdateUserDto, ApplicationUser>();
        }
    }
}
