import { FormEvent, useState } from "react";
import { Api } from "../api/Api";
import { AddProductRequest } from "../api/ApiRequests";

type AddProductFormProps = {
    setRefresh: (b: boolean) => void
}

export function AddProductForm({ setRefresh }: AddProductFormProps) { 
    const [newProduct, setNewProduct] = useState<AddProductRequest>({name: ""});

    const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        Api.AddProduct(newProduct).then(() => setRefresh(true));
    }

    return (
        <form onSubmit={handleSubmit}>
            <h2>Dodaj produkt</h2>
            <label>
                Nazwa produktu:
            </label>
            <div>
                <input type="text" name="name" maxLength={64} required
                    onChange={e => setNewProduct({...newProduct, name: e.target.value})}
                />
            </div>
            <input type="submit" value="Dodaj" />
        </form>
    );
}