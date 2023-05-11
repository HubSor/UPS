import { useEffect, useState } from "react";
import { Api } from "../api/Api";
import { ProductDto } from "../api/Dtos";

type ProductsProps = {
    refresh: boolean
    setRefresh: (b: boolean) => void
}

export function Products({ refresh, setRefresh } : ProductsProps) {
    const [products, setProducts] = useState<ProductDto[]>([]);

    useEffect(() => {
        if (refresh) {
            Api.GetProducts().then(res => setProducts(res.data.products ?? []));
            setRefresh(false);
        } 
    }, [refresh, setRefresh])

    return (
        <div>
            <h2>Produkty</h2>
            {products.map((p, i) => (
                <div>
                    Nr: {i} Name: {p.name} CreatedAt: {p.createdAt.toDateString()}
                </div>
            ))}
        </div>
    );
}