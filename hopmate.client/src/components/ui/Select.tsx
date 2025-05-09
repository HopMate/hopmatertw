import React from 'react';

interface SelectOption {
    value: string;
    label: string;
}

interface SelectProps {
    options: SelectOption[];
    onChange: (event: React.ChangeEvent<HTMLSelectElement>) => void;
    placeholder?: string;
    value?: string;
}

const Select: React.FC<SelectProps> = ({ options, onChange, placeholder, value }) => {
    return (
        <select
            className="w-full border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            onChange={onChange}
            value={value}
        >
            {placeholder && <option value="">{placeholder}</option>}
            {options.map(option => (
                <option key={option.value} value={option.value}>
                    {option.label}
                </option>
            ))}
        </select>
    );
};

export default Select;
